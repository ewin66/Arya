using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Arya.HelperClasses;

namespace Arya.Data
{
    public partial class Sku
    {
        #region Properties (4)

        private string _oldTaxonomy;

        internal IEnumerable<Sku> LinkedFrom
        {
            get { return _SkuLinks1.Where(p => p.Active).Select(sl => sl.Sku); }
        }

        internal IEnumerable<Sku> LinksTo
        {
            get { return _SkuLinks.Where(p => p.Active).Select(sl => sl.Sku1); }
        }

        public string OldTaxonomy
        {
            get
            {
                if (_oldTaxonomy != null)
                    return _oldTaxonomy;
                _oldTaxonomy =
                    SkuInfos.OrderBy(si => si.CreatedOn).Select(si => si.TaxonomyInfo.OldString()).FirstOrDefault()
                    ?? string.Empty;
                return _oldTaxonomy;
            }
        }

        public TaxonomyInfo Taxonomy
        {
            get
            {
                var activeSkuInfo = SkuInfos.FirstOrDefault(si => si != null && si.Active);
                return activeSkuInfo == null ? null : activeSkuInfo.TaxonomyInfo;
                //return SkuInfos.Where(si => si.Active).Select(si => si.TaxonomyInfo).FirstOrDefault();
            }
        }

        #endregion Properties

        #region Methods (5)

        public List<EntityData> GetValuesForAttribute(Attribute attribute, bool useCache = true, bool sortDesc = false)
        {
            if (attribute == null)
                return new List<EntityData>();

            if (attribute.AttributeType == Framework.Data.AryaDb.AttributeTypeEnum.Workflow.ToString())
            {
                var workflowFlag = SkuStates.SingleOrDefault(a => a.Active);
                if (workflowFlag != null)
                {
                    return new List<EntityData>
                           {
                               new EntityData
                               {
                                   Value =
                                       workflowFlag.State.Name
                                       + (workflowFlag.UserApproved == true
                                           ? " (✓)"
                                           : string.Empty)
                               }
                           };
                }
            }

            if (useCache
                && attribute.AttributeType != Framework.Data.AryaDb.AttributeTypeEnum.Derived.ToString()
                && AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.ContainsKeys(attribute, this))
            {
                var x = AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache[attribute, this];
                return x;
            }

            var edGroups = (from ei in EntityInfos
                from ed in ei.EntityDatas
                where ed.Active && ed.AttributeID.Equals(attribute.ID)
                group ed by ed.EntityID
                into edGroup
                select new {EntityId = edGroup.Key, ValueCount = edGroup.Count(), Values = edGroup.ToList()}).ToList();

            foreach (var edGroup in edGroups.Where(grp => grp.ValueCount > 1))
                edGroup.Values.OrderByDescending(ed => ed.CreatedOn).Skip(1).ForEach(ed => ed.Active = false);

            var eds = edGroups.SelectMany(grp => grp.Values.Where(ed => ed.Active)).ToList();

            if (eds.Count > 0)
            {
                var values = EntityData.OrderSkuAttributeValues(eds, sortDesc);
                if (attribute.AttributeType != Framework.Data.AryaDb.AttributeTypeEnum.Derived.ToString())
                    AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.Add(attribute, this, values);
                return values;
            }

            if (attribute.AttributeType == Framework.Data.AryaDb.AttributeTypeEnum.Derived.ToString())
            {
                return new List<EntityData>
                       {
                           new TaxAttValueCalculator(this).ProcessCalculatedAttribute(attribute,
                               Taxonomy)
                       };
            }

            var emptyValues = new List<EntityData>();
            AryaTools.Instance.InstanceData.Dc.SkuAttributeValueCache.Add(attribute, this, emptyValues);
            return emptyValues;
        }

        public List<EntityData> GetValuesForAttribute(string attributeName, bool useCache = true, bool sortDesc = false)
        {
            return string.IsNullOrEmpty(attributeName)
                ? new List<EntityData>()
                : GetValuesForAttribute(Attribute.GetAttributeFromName(attributeName, false), useCache, sortDesc);
        }

        public override string ToString() { return ItemID; }
        // Private Methods (1) 

        partial void OnCreated() { SkuDataDbDataContext.DefaultTableValues(this); }
        // Internal Methods (2) 

        internal bool HasAttribute(string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            return
                EntityInfos.SelectMany(ei => ei.EntityDatas)
                    .Any(ed => ed.Active && ed.Attribute.AttributeName.ToLower().Equals(lowerAttributeName));
        }

        internal bool HasValue(string attributeName, string value)
        {
            var lowerAttributeName = attributeName.ToLower();
            var lowerValue = value.ToLower();
            var containsQuery = false;
            var equalsQuery = false;
            var endsWithQuery = false;
            var startsWithQuery = false;
            if (lowerValue.StartsWith("%") && lowerValue.EndsWith("%"))
            {
                containsQuery = true;
                lowerValue = lowerValue.Substring(1, lowerValue.Length - 2);
            }
            else if (lowerValue.StartsWith("%"))
            {
                endsWithQuery = true;
                lowerValue = lowerValue.Substring(1, lowerValue.Length - 1);
            }
            else if (lowerValue.EndsWith("%"))
            {
                startsWithQuery = true;
                lowerValue = lowerValue.Substring(0, lowerValue.Length - 1);
            }
            else
                equalsQuery = true;

            return
                EntityInfos.SelectMany(ei => ei.EntityDatas)
                    .Any(
                        ed =>
                            ed.Active && ed.Attribute.AttributeName.ToLower().Equals(lowerAttributeName)
                            && ((equalsQuery && ed.Value.ToLower().Equals(lowerValue))
                                || (containsQuery && ed.Value.ToLower().Contains(lowerValue))
                                || (startsWithQuery && ed.Value.ToLower().StartsWith(lowerValue))
                                || (endsWithQuery && ed.Value.ToLower().Contains(lowerValue))));
        }

        public void UpsertValue(string attributeName, string value)
        {
            var existingEds = GetValuesForAttribute(attributeName, false);
            EntityData ed = null;
            if (existingEds.Count > 0)
            {
                if (existingEds.Count == 1 && existingEds[0].Value.Equals(value))
                    return;

                foreach (var existingEd in existingEds)
                {
                    ed = existingEd;
                    ed.Active = false;
                }
            }

            if (string.IsNullOrEmpty(value))
                return;

            var newEd = new EntityData {Attribute = Attribute.GetAttributeFromName(attributeName, true), Value = value};
            if (ed != null)
                ed.EntityInfo.EntityDatas.Add(newEd);
            else
                EntityInfos.Add(new EntityInfo {EntityDatas = {newEd}});
        }

        #endregion Methods

    }
}