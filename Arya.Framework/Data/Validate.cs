using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Math;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;

namespace Arya.Framework.Data
{
    public class Validate
    {
        #region Fields (1)

        private readonly Regex _integerValidation = new Regex(@"^[+-]?\d+$", RegexOptions.Compiled);
        private readonly AryaDbDataContext _db;

        #endregion Fields

        #region Constructor

        public Validate(AryaDbDataContext db)
        {
            _db = db;
        }

        #endregion

        #region Methods (1)

        // Public Methods (1) 

        public bool IsValidDataType(EntityData ed, SchemaData scd)
        {
            if (ed.EntityID == Guid.Empty && ed.AttributeID == Guid.Empty)
                return true;

            var validLovs = GetLovs(ed.Attribute, ed.Sku.Taxonomy, ed.Sku);
            if (validLovs != null)
                return validLovs.Contains(ed.Value);

            int decimalDataType;

            if (scd.DataType.ToLower() == "text")
                return true;

            if (scd.DataType.ToLower().StartsWith("int") || scd.DataType.Equals("0"))
                return _integerValidation.IsMatch(ed.Value);

            if (Int32.TryParse(scd.DataType, out decimalDataType))
            {
                var decimalValidation = new Regex(@"^[+-]?\d+[.]\d{" + decimalDataType + "}$");
                var x = decimalValidation.IsMatch(ed.Value);
                return x;
            }

            if (scd.DataType.ToLower() == "fraction")
            {
                double result;
                return _integerValidation.IsMatch(ed.Value) || MathUtils.IsFract(ed.Value, out result);
            }

            if (scd.DataType.ToLower() == "boolean")
                return ed.Value == "Yes" || ed.Value == "No";

            if (scd.DataType.ToLower() == "number")
            {
                double result;
                return MathUtils.TryConvertToNumber(ed.Value, out result);
            }

            if (scd.DataType.ToLower() == "lov")
                return scd.SchemaInfo.ListOfValues.Where(a => a.Active).Select(v => v.Value).Contains(ed.Value);

            if (scd.DataType.ToLower() == "numeric text")
                return true;

            return false;
        }

        private IEnumerable<EntityData> GetParentAttributeValues(SchemaInfo si, Sku sku)
        {
            var smi =
                si.SchemaMetaInfos.FirstOrDefault(
                    a =>
                        a.Attribute.AttributeName == "Dependent Attribute"
                        && a.Attribute.AttributeType
                        == AttributeTypeEnum.SchemaMeta.ToString());

            if (smi == null) // Dependent Meta Attirbute has not been defined
                return null;

            var smd = smi.SchemaMetaDatas.SingleOrDefault(a => a.Active);

            if (smd == null) // No active Metavalues for Depedendent Attributes
                return null;

            var parentAttributeName = smd.Value.ToLower().Trim();

            return String.IsNullOrEmpty(parentAttributeName) ? null : sku.GetValuesForAttribute(_db, parentAttributeName);
        }

        public IEnumerable<string> GetLovs(Attribute attribute, TaxonomyInfo taxonomy, Sku sku = null)
        {
            if (attribute == null || taxonomy == null)
                return null;

            IEnumerable<string> lov;
            var node = taxonomy;

            do
            {
                lov = GetLovsForNode(attribute, node, sku);
                node = node.TaxonomyData.ParentTaxonomyInfo;
            } while (lov == null && node != null);

            return lov;
        }

        private IEnumerable<string> GetLovsForNode(Attribute attribute, TaxonomyInfo taxonomy, Sku sku = null)
        {
            if (attribute == null || taxonomy == null)
                return null;

            var si = taxonomy.SchemaInfos.FirstOrDefault(s => s.Attribute == attribute);

            if (si == null)
                return null;

            var lovs = si.ListOfValues.Where(a => a.Active).ToList();

            if (lovs.Count == 0)
                return null;

            if (sku == null)
                return lovs.Select(a => a.Value).Distinct();

            //Get LOVs where parent Value is null                    
            var defaultLov =
                lovs.Where(a => a.ParentValue == null || a.ParentValue == "Ø").Select(v => v.Value).Distinct().ToList();

            // Combinations:
            // value: parentValue == x:null -- NA
            // value: parentValue == x:y --- if x is the cell value then y must be the parentValue -- check first
            // value: parentValue == x:* --- there must exist a parent value for cell value x -- check second
            // value: parentValue == x:Ø --- cell Value must be x when parentValue does not exist -- check third
            // value: parentValue == *:y --- Any/other cell value must have parentValue y  -- check fourth
            // value: parentValue == *:Ø --- No parent should exist for any/other value -- fifth check
            // value: parentValue == *:* --- all the values must have parent value -- sixth check
            // value: parentValue == Ø:y --- Does not make sense ..

            var parentEntities = GetParentAttributeValues(si, sku);
            if (parentEntities == null) //No Dependencies
                return defaultLov;

            IEnumerable<string> parentAttributeValues = parentEntities.Select(v => v.Value).ToList();
            if (!parentAttributeValues.Any())
                return defaultLov;

            var allowedLovs =
                lovs.Where(a => a.ParentValue == "✽" || parentAttributeValues.Contains(a.ParentValue))
                    .Select(v => v.Value)
                    .Distinct()
                    .ToList();

            if (allowedLovs.Contains("Ø"))
                return new List<string>();

            if (allowedLovs.Count == 0 || allowedLovs.Contains("✽"))
                return null;

            return allowedLovs;
        }
    }

    #endregion Methods
}