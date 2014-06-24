using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinqKit;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;

namespace Arya.Framework.Data.AryaDb
{

    public partial class TaxonomyInfo : BaseEntity
    {
        private const string BlankValue = "[blank]";
        public const string DELIMITER = ">";
        public const string NodeTypeDerived = "Derived";
        public const string NodeTypeRegular = "Regular";

        public const string CROSS_PREFIX = " -> CL From -> ";

        public TaxonomyInfo(AryaDbDataContext parentContext, bool initialize = true)
            : this()
        {
            ParentContext = parentContext;
            Initialize = initialize;
            InitEntity();
        }



        public IQueryable<Sku> GetSkus(bool exportCrossListNodes)
        {
            IQueryable<Sku> allSkus;
            if (NodeType == NodeTypeDerived)
            {
                if (!exportCrossListNodes)
                    allSkus = Enumerable.Empty<Sku>().AsQueryable();
                else
                {
                    var query = new Query(ParentContext.CurrentProject.ID, ParentContext.CurrentUser.ID);
                    var cl = query.FetchCrossListObject(this);
                    if (cl == null)
                        allSkus = Enumerable.Empty<Sku>().AsQueryable();
                    else
                    {
                        allSkus =
                            query.GetFilteredSkus(cl.TaxonomyIDFilter, cl.ValueFilters, cl.AttributeTypeFilters,
                                cl.MatchAllTerms).Distinct();
                    }
                }
            }
            else
            {
                allSkus = from sku in ParentContext.Skus
                          where sku.SkuInfos.Any(si => si.Active && si.TaxonomyInfo == this)
                          select sku;
            }
            return allSkus;
        }

        public List<TaxonomyInfo> AllChildren
        {
            get
            {
                var allChildren = new List<TaxonomyInfo>();
                var children = ChildTaxonomyDatas.Where(td => td.Active).Select(td => td.TaxonomyInfo);
                children.ForEach(child =>
                {
                    allChildren.Add(child);
                    allChildren.AddRange(child.AllChildren);
                });
                return allChildren;
            }
        }

        public User _user;
        public User CurrentUser
        {
            get { return _user; }
            set { _user = value; }
        }

        public Project _project;
        public Project CurrentProject
        {
            get { return _project; }
            set { _project = value; }
        }

        public EntitySet<TaxonomyData> ChildTaxonomyDatas
        {
            get { return TaxonomyDatas1; }
            set { TaxonomyDatas1 = value; }
        }

        public bool IsLeafNode
        {
            get { return !ChildTaxonomyDatas.Any(tax => tax.Active); }
        }

        /// <summary>
        /// Gets all leaf children, including the source if its a leaf.
        /// </summary>
        public IEnumerable<TaxonomyInfo> AllLeafChildren
        {
            get
            {
                if (IsLeafNode)
                {
                    return
                        ChildTaxonomyDatas.Where(p => p.Active)
                            .Traverse(p => p.TaxonomyInfo.ChildTaxonomyDatas.Where(q => q.Active))
                        //                            .Where(p => p.TaxonomyInfo.SkuInfos.Any(q => q.Active))
                            .Select(p => p.TaxonomyInfo)
                            .Union(this);
                }

                return
                    ChildTaxonomyDatas.Where(p => p.Active)
                        .Traverse(p => p.TaxonomyInfo.ChildTaxonomyDatas.Where(q => q.Active))
                    //                        .Where(p => p.TaxonomyInfo.SkuInfos.Any(q => q.Active))
                        .Select(p => p.TaxonomyInfo);
            }
        }

        public int SkuCount
        {
            get { return SkuInfos.Count(si => si.Active); }
        }

        public TaxonomyData TaxonomyData
        {
            get { return TaxonomyDatas.FirstOrDefault(td => td.Active); }
        }

        public bool HasSkus
        {
            get { return SkuInfos.Any(si => si.Active); }
        }

        public string NodeName
        {
            get { return TaxonomyData.NodeName; }
        }

        public override string ToString()
        {
            return ToString(DELIMITER);
        }

        public string ToString(string delimiter)
        {
            var taxonomyData = TaxonomyData;
            return taxonomyData == null
                ? String.Empty
                : (taxonomyData.ParentTaxonomyInfo == null
                    ? taxonomyData.NodeName
                    : String.Format("{1}{0}{2}", delimiter, taxonomyData.ParentTaxonomyInfo,
                        String.IsNullOrEmpty(taxonomyData.NodeName) ? BlankValue : taxonomyData.NodeName));
        }

        public string ToString(bool noT1, string delimiter = DELIMITER)
        {
            var originalString = ToString();

            if (!noT1)
                return originalString;

            var delimLocation = originalString.IndexOf(delimiter, StringComparison.InvariantCulture);
            var returnString = delimLocation < 0 ? String.Empty : originalString.Substring(delimLocation + 1);

            return returnString;
        }

        public string OldString()
        {
            var oldTaxData = TaxonomyDatas.OrderBy(td => td.CreatedOn).FirstOrDefault();
            return oldTaxData == null
                ? String.Empty
                : (oldTaxData.ParentTaxonomyID == null
                    ? oldTaxData.NodeName
                    : String.Format("{1}{0}{2}", DELIMITER, oldTaxData.ParentTaxonomyInfo.OldString(),
                        oldTaxData.NodeName));
        }


        public IEnumerable<string> ToStringParts()
        {
            if (TaxonomyData == null)
                yield break;

            if (TaxonomyData.ParentTaxonomyInfo != null)
            {
                foreach (var part in TaxonomyData.ParentTaxonomyInfo.ToStringParts())
                    yield return part;
            }

            yield return NodeName;
        }
        public int GetNodeDepth()
        {
            if (TaxonomyData == null)
                return 0;

            if (TaxonomyData.ParentTaxonomyInfo == null)
                return 1;

            return TaxonomyData.ParentTaxonomyInfo.GetNodeDepth() + 1;
        }

        public static int GetNodeCount(TaxonomyInfo taxonomy)
        {
            IEnumerable<TaxonomyInfo> childTaxonomyInfos;
            int count = 0;
            if (taxonomy != null)
            {
                childTaxonomyInfos = taxonomy.ChildTaxonomyDatas.Where(td => td.Active).Select(td => td.TaxonomyInfo).ToList();
                count = childTaxonomyInfos.Count();
            }
            //else
            //    childTaxonomyInfos = GetLevel1Nodes().ToList();

            //count += childTaxonomyInfos.Sum(childNode => GetNodeCount(childNode));

            return count;
        }


        public TaxonomyData OldTaxonomyData
        {
            get { return TaxonomyDatas.OrderBy(td => td.CreatedOn).FirstOrDefault(); }
        }
        #region IComparable Members

        public int CompareTo(object other) { return String.CompareOrdinal(ToString(), other.ToString()); }

        #endregion
    }
}