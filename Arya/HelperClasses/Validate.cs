using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Arya.Data;
using Arya.Framework.Math;
using Attribute = Arya.Data.Attribute;

namespace Arya.HelperClasses
{
    public static class Validate
    {
        #region Fields (1)

        private static readonly Regex IntegerValidation = new Regex(@"^[+-]?\d+$", RegexOptions.Compiled);
        //private static HashSet<string> validUoms;

        #endregion Fields

        #region Methods (1)

        // Public Methods (1) 

        public static bool IsValidDataType(EntityData ed, SchemaData scd)
        {
            //bool isDependencySatisfied = CheckDependency(ed, scd);

            //if (!isDependencySatisfied)
            //    return false;
            if (ed.EntityID == Guid.Empty && ed.AttributeID == Guid.Empty)
                return true;

            var validLovs = GetLovs(ed.Attribute, ed.Sku.Taxonomy, ed.Sku);
            if (validLovs != null)
                return validLovs.Contains(ed.Value);

            int decimalDataType;

            if (scd.DataType.ToLower() == "text")
                return true;

            if (scd.DataType.ToLower().StartsWith("int") || scd.DataType.Equals("0"))
                return IntegerValidation.IsMatch(ed.Value);

            if (Int32.TryParse(scd.DataType, out decimalDataType))
            {
                var decimalValidation = new Regex(@"^[+-]?\d+[.]\d{" + decimalDataType + "}$");
                var x = decimalValidation.IsMatch(ed.Value);
                return x;
            }

            if (scd.DataType.ToLower() == "fraction")
            {
                double result;
                return IntegerValidation.IsMatch(ed.Value) || MathUtils.IsFract(ed.Value, out result);
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

        //private static bool CheckDependency(EntityData ed, SchemaData scd)
        //{
        //    var checkValue = ed.Value;
        //    var si = scd.SchemaInfo;
        //    var lovs = si.ListOfValues;

        //    if (lovs == null)
        //        return true; // No Depenedencies

        //    var parentValueforCheckValue = lovs.Where(v => v.Value == checkValue && v.Active).Select(p => p.ParentValue).FirstOrDefault();
        //    var parentValueForAnyCheckValue = lovs.Where(v => v.Value == "✽" && v.Active).Select(p => p.ParentValue).FirstOrDefault();

        //    if (lovs.Any(ch => ch.Value == checkValue && ch.ParentValue == null && ch.Active))
        //        return true;

        //    //Combinations:
        //    // value: parentValie == x:null -- NA
        //    // value: parentValue == x:y --- if x is the cell value then y must be the parentValue -- check first
        //    // value: parentValue == x:* --- there must exist a parent value for cell value x -- check second
        //    // value: parentValue == x:Ø --- cell Value must be x when parentValue does not exist -- check third
        //    // value: parentValue == *:y --- Any/other cell value must have parentValue y  -- check fourth
        //    // value: parentValue == *:Ø --- No parent should exist for any/other value -- fifth check
        //    // value: parentValue == *:* --- all the values must have parent value -- sixth check
        //    // value: parentValue == Ø:y --- Does not make sense ..

        //    var parentAttributeValues = GetParentAttributeValues(si, ed.Sku);
        //    if (parentAttributeValues == null) // Dependent Meta-attribute Field is not defined or is Empty. So, No Dependency
        //        return true; 

        //    if (parentValueforCheckValue == null && parentValueForAnyCheckValue == null) //Check Value or * is not present in LoV View
        //        return false;

        //    bool isParentValueForCheckValueMatch = parentAttributeValues.Select(v => v.Value).Contains(parentValueforCheckValue); // x:Y 

        //    if (parentValueforCheckValue != null && parentValueforCheckValue != "✽" && parentValueforCheckValue != "Ø" && !isParentValueForCheckValueMatch)
        //        return false;

        //    if (!isParentValueForCheckValueMatch)
        //    {
        //        if (parentValueforCheckValue == "✽" && parentAttributeValues.Count > 0) // x:*
        //            return true;
        //        if (parentValueforCheckValue == "✽" && parentAttributeValues.Count == 0)
        //            return false;

        //        if (parentValueforCheckValue == "Ø" && parentAttributeValues.Count == 0) // x:Ø
        //            return true;
        //        if (parentValueforCheckValue == "Ø" && parentAttributeValues.Count > 0)
        //            return false;

        //        if (parentValueForAnyCheckValue != null) // *:y
        //            if (parentAttributeValues.Any(v => v.Value.ToLower().Trim() == parentValueForAnyCheckValue.ToLower().Trim()))
        //                return true;

        //        if (parentValueForAnyCheckValue == "Ø" && parentAttributeValues.Count == 0) // *:Ø
        //            return true;

        //        if (parentValueForAnyCheckValue == "✽" && parentAttributeValues.Count > 0)
        //            return true;
        //    }

        //    return isParentValueForCheckValueMatch;            
        //}

        private static IEnumerable<EntityData> GetParentAttributeValues(SchemaInfo si, Sku sku)
        {
            var smi =
                si.SchemaMetaInfos.FirstOrDefault(
                    a =>
                        a.Attribute.AttributeName == "Dependent Attribute"
                        && a.Attribute.AttributeType
                        == Framework.Data.AryaDb.AttributeTypeEnum.SchemaMeta.ToString());

            if (smi == null) // Dependent Meta Attirbute has not been defined
                return null;

            var smd = smi.SchemaMetaDatas.SingleOrDefault(a => a.Active);

            if (smd == null) // No active Metavalues for Depedendent Attributes
                return null;

            var parentAttributeName = smd.Value.ToLower().Trim();

            return String.IsNullOrEmpty(parentAttributeName) ? null : sku.GetValuesForAttribute(parentAttributeName);
        }

        public static IEnumerable<string> GetLovs(Attribute attribute, TaxonomyInfo taxonomy, Sku sku = null)
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

        private static IEnumerable<string> GetLovsForNode(Attribute attribute, TaxonomyInfo taxonomy, Sku sku = null)
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
            if (parentAttributeValues.Count() == 0)
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

        internal static bool ValidateUom(string uom)
        {
            //if (validUoms == null)
            //    //validUoms = new HashSet<string>(AryaTools.Instance.InstanceData.Dc.ProjectUoms.Select(u => u.Uom).Distinct());

            //return validUoms.Contains(uom);
            return true;
        }
    }

    #endregion Methods
}