using System.Collections.Generic;
using System.Linq;
using LinqKit;

namespace Arya.Framework.Data.AryaDb
{
    public partial class Sku : BaseEntity
    {
        #region ItemType enum

        public enum ItemType
        {
            Product,
            EnrichmentImage
        }

        #endregion

        public Sku(AryaDbDataContext parentContext, bool initialize = true)
            : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }

        partial void OnCreated()
        {
            _derivedValueCalculator = new TaxAttValueCalculator(this);
        }

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

        // Public Methods (2)

        private TaxAttValueCalculator _derivedValueCalculator;

        public List<EntityData> GetValuesForAttribute(AryaDbDataContext db, Attribute attribute, bool useCache = true,
            bool sortDesc = false)
        {
            if (attribute == null)
                return new List<EntityData>();

            if (useCache && db.SkuAttributeValueCache.ContainsKeys(attribute, this))
            {
                var x = db.SkuAttributeValueCache[attribute, this];
                return x;
            }

            var edGroups = (from ei in EntityInfos
                            from ed in ei.EntityDatas
                            where ed.Active && ed.AttributeID.Equals(attribute.ID)
                            group ed by ed.EntityID
                                into edGroup
                                select new { EntityId = edGroup.Key, ValueCount = edGroup.Count(), Values = edGroup.ToList() }).ToList();

            foreach (var edGroup in edGroups.Where(grp => grp.ValueCount > 1))
                edGroup.Values.OrderByDescending(ed => ed.CreatedOn).Skip(1).ForEach(ed => ed.Active = false);

            var eds = edGroups.SelectMany(grp => grp.Values.Where(ed => ed.Active)).ToList();

            if (eds.Count > 0)
            {
                var values = EntityData.OrderSkuAttributeValues(eds, sortDesc);
                db.SkuAttributeValueCache.Add(attribute, this, values);
                return values;
            }

            if (attribute.Type == AttributeTypeEnum.Derived)
            {
                var processCalculatedAttribute = _derivedValueCalculator.ProcessCalculatedAttribute(attribute, Taxonomy);
                return new List<EntityData>
                       {
                           new EntityData
                           {
                               Value =
                                   processCalculatedAttribute
                           }
                       };
            }

            var emptyValues = new List<EntityData>();
            db.SkuAttributeValueCache.Add(attribute, this, emptyValues);
            return emptyValues;
        }

        public List<EntityData> GetValuesForAttribute(AryaDbDataContext db, string attributeName,
            bool useCache = true, bool sortDesc = false)
        {
            return string.IsNullOrEmpty(attributeName)
                ? new List<EntityData>()
                : GetValuesForAttribute(db, Attribute.GetAttributeFromName(db, attributeName, false), useCache, sortDesc);
        }

        public override string ToString() { return ItemID; }

        // Internal Methods (2) 

        public bool HasAttribute(string attributeName)
        {
            var lowerAttributeName = attributeName.ToLower();
            return
                EntityInfos.SelectMany(ei => ei.EntityDatas)
                    .Any(ed => ed.Active && ed.Attribute.AttributeName.ToLower().Equals(lowerAttributeName));
        }

        public bool HasValue(string attributeName, string value)
        {
            var lowerAttributeName = attributeName.ToLower();
            var lowerValue = value.ToLower();

            var entityDatas = EntityInfos.SelectMany(ei => ei.EntityDatas);

            if (lowerValue.StartsWith("%") && lowerValue.EndsWith("%"))
            {
                lowerValue = lowerValue.Substring(1, lowerValue.Length - 2);
                return
                    entityDatas.Any(
                        ed =>
                            ed.Active && ed.Attribute.AttributeName.ToLower().Equals(lowerAttributeName)
                            && ed.Value.ToLower().Contains(lowerValue));
            }

            if (lowerValue.StartsWith("%"))
            {
                lowerValue = lowerValue.Substring(1, lowerValue.Length - 1);
                return
                    entityDatas.Any(
                        ed =>
                            ed.Active && ed.Attribute.AttributeName.ToLower().Equals(lowerAttributeName)
                            && ed.Value.ToLower().Contains(lowerValue));
            }

            if (lowerValue.EndsWith("%"))
            {
                lowerValue = lowerValue.Substring(0, lowerValue.Length - 1);
                return
                    entityDatas.Any(
                        ed =>
                            ed.Active && ed.Attribute.AttributeName.ToLower().Equals(lowerAttributeName)
                            && ed.Value.ToLower().StartsWith(lowerValue));
            }

            //equals query
            return
                entityDatas.Any(
                    ed =>
                        ed.Active && ed.Attribute.AttributeName.ToLower().Equals(lowerAttributeName)
                        && ed.Value.ToLower().Equals(lowerValue));
        }

        public void UpsertValue(AryaDbDataContext db, string attributeName, string value)
        {
            var existingEds = GetValuesForAttribute(db, attributeName, false);
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

            var newEd = new EntityData(db)
            {
                Attribute =
                    Attribute.GetAttributeFromName(db, attributeName, true,
                        AttributeTypeEnum.Sku),
                Value = value
            };
            if (ed != null)
                ed.EntityInfo.EntityDatas.Add(newEd);
            else
                EntityInfos.Add(new EntityInfo(db) { EntityDatas = { newEd } });
        }

        #endregion Methods
    }
}