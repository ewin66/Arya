using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.Properties;
using Arya.HelperClasses;
using Arya.HelperForms;
using Arya.UserControls;
using SchemaAttribute = Arya.HelperClasses.SchemaAttribute;
using Arya.SpellCheck;

namespace Arya.Data
{
    public class AttributeColumn : IComparable<AttributeColumn>,ISpell
    {
        private KeyValuePair<bool, string>? _updatable = new KeyValuePair<bool, string>(true, "");
        KeyValuePair<bool, string> ISpell.Updatable
        {
            get
            {
                return (KeyValuePair<bool, string>)_updatable;
            }
        }
        public static bool SortDisplayOrders = true;
        public static bool SortNavigationOrders = true;

        private readonly Dictionary<AttributeColumn, decimal[]> _comparerCache =
            new Dictionary<AttributeColumn, decimal[]>();

        internal List<SchemaData> _schemaDatas;
        private List<TaxonomyInfo> _taxonomies;
        private List<TaxonomyInfo> taxinfos;
        private IEnumerable<EntityData> entities;
        private IEnumerable<List<TaxonomyInfo>> enumerable;
        private IQueryable<EntityData> queryable;

        public AttributeColumn(Attribute attribute, IEnumerable<TaxonomyInfo> taxonomies, IQueryable<EntityInfo> entities)
        {
            Entities = entities;
            Attribute = attribute;
            Taxonomies.AddRange(taxonomies);
        }

      

       

        public string AttributeType
        {
            get
            {
                if (Attribute.Type.ToString() == "Sku")
                {
                    return "Product";
                }
                return Attribute.Type.ToString();
            }
        }

        public string AttributeGroup
        {
            get { return Attribute.Group; }
        }

        public Attribute Attribute { get; set; }

        public List<TaxonomyInfo> Taxonomies
        {
            get
            {
                if (_taxonomies == null)
                    _taxonomies = new List<TaxonomyInfo>();

                return _taxonomies;
            }
            set { _taxonomies = value; }
        }

        public TaxonomyInfo Taxonomy
        {
            get { return Taxonomies.Count == 1 ? Taxonomies.First() : null; }
        }

        public IEnumerable<EntityInfo> Entities { get; set; }

        [TypeConverter(typeof (BooleanToYesOrBlankConverter))]
        public bool IsMultiValued { get; set; }

        public int Position { get; set; }

        public int ColumnWidth { get; set; }

        public bool Visible { get; set; }

        public int? NavigationOrder
        {
            get { return SchemaDatas.Count != 1 ? null : (int?) SchemaDatas.First().NavigationOrder; }
        }

        public int? DisplayOrder
        {
            get { return SchemaDatas.Count != 1 ? null : (int?) SchemaDatas.First().DisplayOrder; }
        }

        public CheckState IsRanked
        {
            get
            {
                var isRankedTaxCount = SchemaDatas.Count(sd => sd.NavigationOrder > 0 || sd.DisplayOrder > 0);

                return isRankedTaxCount == 0
                    ? CheckState.Unchecked
                    : (isRankedTaxCount == Taxonomies.Count ? CheckState.Checked : CheckState.Indeterminate);
            }
        }

        private List<SchemaData> SchemaDatas
        {
            get
            {
                return _schemaDatas ?? (_schemaDatas = (from si in Attribute.SchemaInfos
                    where Taxonomies.Contains(si.TaxonomyInfo)
                    from sd in si.SchemaDatas
                    where sd.Active
                    select sd).ToList());
            }
        }

        public CheckState InSchema
        {
            get
            {
                if (Taxonomies.Count == 0)
                    return CheckState.Unchecked;

                var inSchemaSchemaDatas = SchemaDatas.Count(sd => sd.InSchema);

                return inSchemaSchemaDatas == 0
                    ? CheckState.Unchecked
                    : (inSchemaSchemaDatas == Taxonomies.Count ? CheckState.Checked : CheckState.Indeterminate);
            }
        }

        public string AttributeName
        {
            get
            {
                try
                {
                    return Attribute.AttributeName;
                }
                catch (NullReferenceException)
                {
                    return string.Empty;
                }
            }
            set { SetAttributeName(value); }
        }

        #region IComparable<AttributeColumn> Members

        public int CompareTo(AttributeColumn other)
        {
            var xComparerValues = GetComparerValue(this);
            var yComparerValues = GetComparerValue(other);

            return xComparerValues[0] != yComparerValues[0]
                ? xComparerValues[0].CompareTo(yComparerValues[0])
                : (xComparerValues[1] != yComparerValues[1]
                    ? xComparerValues[1].CompareTo(yComparerValues[1])
                    : String.Compare(Attribute.AttributeName, other.Attribute.AttributeName, StringComparison.Ordinal));
        }

        #endregion

        public override string ToString()
        {
            return Attribute.AttributeName + (Taxonomy != null ? " - " + Taxonomy : string.Empty);
        }

        public static void RefreshAttributeColumnPositions(List<AttributeColumn> attributeColumns,
            Dictionary<string, Attribute> allAttributes, List<TaxonomyInfo> taxonomies, bool firstTime)
        {
            var attributePrefs = AryaTools.Instance.InstanceData.CurrentUserProjectsPreferences;
            var attIds = attributeColumns.Select(ac => ac.Attribute.ID).ToHashSet();

            var toAdd = (from att in allAttributes
                where
                    ((att.Value.Type == AttributeTypeEnum.Sku || att.Value.Type == AttributeTypeEnum.Derived
                      || att.Value.Type == AttributeTypeEnum.Flag || att.Value.Type == AttributeTypeEnum.Global)
                     && !attIds.Contains(att.Value.ID) && !att.Value.AttributeName.Equals(Resources.NewAttributeName))
                select
                    new AttributeColumn(att.Value, taxonomies, null)
                    {
                        Attribute = att.Value,
                        ColumnWidth = EntityDataGridView.DefaultColumnWidth,
                        Visible = !firstTime
                    }).ToList();

            attributeColumns.AddRange(toAdd);

            if (firstTime && (taxonomies != null))
            {
                foreach (var ac in attributeColumns)
                {
                    ac.Visible = (ac.Visible ||
                                  (ac.InSchema != CheckState.Unchecked && attributePrefs.InSchemaAttributes) ||
                                  (ac.IsRanked != CheckState.Unchecked && attributePrefs.RankedAttributes) ||
                                  (ac.Attribute.Type == AttributeTypeEnum.Global && attributePrefs.GlobalAttributes) ||
                                  (ac.Attribute.Type == AttributeTypeEnum.Sku && attributePrefs.ProductAttributes)||
                                  attributePrefs.AttributeGroupInclusions.Contains(ac.AttributeGroup)||
                                  attributePrefs.AttributeCustomInclusions.Contains(ac.AttributeName)
                                 );

                    //bool treated = false;

                    ////InSchema User Preference
                    //if (ac.InSchema != CheckState.Unchecked)
                    //{
                    //    ac.Visible = (attributePrefs.InSchemaAttributes || ac.Visible);
                    //    treated = true;
                    //}

                    ////IsRanked User Preference
                    //if (ac.IsRanked != CheckState.Unchecked)
                    //{
                    //    ac.Visible = (attributePrefs.RankedAttributes || ac.Visible);
                    //    treated = true;
                    //}

                    ////Global User Preference
                    //if (ac.Attribute.Type == AttributeTypeEnum.Global)
                    //{
                    //    ac.Visible = (attributePrefs.GlobalAttributes || ac.Visible);
                    //    treated = true;
                    //}

                    ////Extended User Preference
                    //if (!treated)
                    //    ac.Visible = (attributePrefs.ProductAttributes || ac.Visible);

                    //if (attributePrefs.AttributeGroupInclusions.Contains(ac.AttributeGroup))
                    //    ac.Visible = true;

                    //if (attributePrefs.AttributeCustomInclusions.Contains(ac.AttributeName))
                    //    ac.Visible = true;
                }
            }

            attributeColumns.Sort();

            for (var i = 0; i < attributeColumns.Count; i++)
                attributeColumns[i].Position = i;
        }

        private void SetAttributeName(string value)
        {
            if (Entities == null || (value == Attribute.AttributeName))
                return;

            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show(@"AttributeName cannot be renamed to blank");
                return;
            }

            if (Attribute.Type.ToString().Contains("Meta") || Attribute.Type == AttributeTypeEnum.Workflow)
            {
                MessageBox.Show(string.Format("{0} attributes cannot be renamed", Attribute.Type));
                return;
            }

            var oldAttribute = Attribute;
            var newAtt = Attribute.GetAttributeFromName(value, true, Attribute.Type);
            if (newAtt.Type != oldAttribute.Type)
            {
                MessageBox.Show(string.Format("{0} attribute cannot be renamed to a {1} type.", Attribute.Type,
                    newAtt.Type));
                return;
            }

            var waitkey = FrmWaitScreen.ShowMessage("Renaming attribute");

            UpdateDerivedAttributeExpressions(newAtt, oldAttribute);
            UpdateEntities(newAtt, waitkey);
            _schemaDatas = null; //Recompute the SchemaDatas!
            Attribute = newAtt;

            AryaTools.Instance.SaveChangesIfNecessary(false, false);
            FrmWaitScreen.HideMessage(waitkey);
        }

        private void UpdateEntities(Attribute newAtt, Guid waitkey)
        {
            //Update Entities
            //var eds = (from ei in Entities
            //    join ed in AryaTools.Instance.InstanceData.Dc.EntityDatas on ei.ID equals ed.EntityID
            //    where ed.AttributeID == Attribute.ID && ed.Active
            //    select new {ed, ei}).ToList();


            var eds =
                Entities.SelectMany(
                    ei => ei.EntityDatas.Where(ed => ed != null && (ed.AttributeID == Attribute.ID && ed.Active)),(ei,ed)=>new {ei,ed}).ToList();
                        

            var itemCount = eds.Count;
            var iCtr = 0;

            //DateTime.UtcNow is 20-30x faster than DateTime.Now as it skips local time conversion
            var lastUpdated = DateTime.UtcNow;
            foreach (var rec in eds)
            {
                var ed = rec.ed;
                ed.Active = false;

                rec.ei.EntityDatas.Add(new EntityData
                                       {
                                           Attribute = newAtt,
                                           Field1 = ed.Field1,
                                           Field2 = ed.Field2,
                                           Field3 = ed.Field3,
                                           Field4 = ed.Field4, //Field5orStatus = ed.Field5OrStatus,
                                           Value = ed.Value,
                                           Uom = ed.Uom
                                       });

                ++iCtr;
                if (DateTime.UtcNow.Subtract(lastUpdated).TotalMilliseconds > 500)
                {
                    FrmWaitScreen.UpdateMessage(waitkey,
                        string.Format("Renaming attribute ({0} of {1} values)", iCtr, itemCount));
                    lastUpdated = DateTime.UtcNow;
                }
            }
        }

        private static void UpdateDerivedAttributeExpressions(Attribute newAtt, Attribute oldAttribute)
        {
            //Update Derived Attribute Expressions
            var newAttDaList = newAtt.DerivedAttributes.Select(p => p.ID).ToList();

            var derivedAttributes = oldAttribute.DerivedAttributes.Where(p => !newAttDaList.Contains(p.ID)).ToList();
            foreach (var a in derivedAttributes)
            {
                var da = new DerivedAttribute
                         {
                             AttributeID = newAtt.ID,
                             Expression = a.Expression,
                             MaxResultLength = a.MaxResultLength,
                             TaxonomyID = a.TaxonomyID
                         };
                newAtt.DerivedAttributes.Add(da);
            }
        }

        private decimal[] GetComparerValue(AttributeColumn attCol)
        {
            if (_comparerCache.ContainsKey(attCol))
                return _comparerCache[attCol];

            var majorRank = 0;
            decimal minorRank = 0;
            try
            {
                decimal navOrder, dispOrder;
                bool inSchema;
                SchemaAttribute.GetAttributeOrders(attCol.Attribute, attCol.Taxonomy, out navOrder, out dispOrder,
                    out inSchema);

                if (navOrder > 0 && SortNavigationOrders)
                {
                    majorRank = 2;
                    minorRank = navOrder;
                }
                else if (dispOrder > 0 && SortDisplayOrders)
                {
                    majorRank = 3;
                    minorRank = dispOrder;
                }
                else if (attCol.Attribute.Type == AttributeTypeEnum.Global)
                {
                    majorRank = 0;
                    minorRank = 0;
                }
                else if (attCol.Attribute.Type == AttributeTypeEnum.Derived)
                {
                    majorRank = 1;
                    minorRank = 0;
                }
                else if (inSchema)
                {
                    majorRank = 4;
                    minorRank = 0;
                }
                else
                {
                    majorRank = 5;
                    minorRank = 0;
                }

                //if (!columnProperty.Visible || !AttributeHasValueInSkus(columnProperty.Attribute))
                //    majorRank += 10;
            }
            catch (Exception)
            {
            }

            var ranks = new[] {majorRank, minorRank};
            _comparerCache.Add(attCol, ranks);
            return ranks;
        }

        string ISpell.GetType()
        {
            return this.GetType().ToString();
        }

        Dictionary<string, string> ISpell.GetSpellValue()
        {
            var   _propertyNameValue = new Dictionary<string, string>();
            _propertyNameValue = new Dictionary<string, string>();
            _propertyNameValue.Add("AttributeName", this.AttributeName);  
            return _propertyNameValue;
        }

        Guid ISpell.GetId()
        {
            return  Attribute.ID;
        }

        ISpell ISpell.SetValue(string propertyName,string value)
        {
           
            if (propertyName.ToLower() == "attributename")
            {                
                AttributeName = value;               
            }
            return this;
        }

        string ISpell.GetLocation()
        {
            return "Test location for attribute view.";
        }
    }
}