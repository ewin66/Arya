using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using LinqKit;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Extensions;
using Arya.Framework.Properties;
using Arya.HelperClasses;
using Arya.SpellCheck;

namespace Arya.Data
{
    // This partial part of the Taxonomy Info class contains helper methods to 
    // manage attribute orders - insert new ones and reorder existing orders
    public partial class TaxonomyInfo : IComparable, ISpell
    {
        #region Fields (5)

        public const string NodeTypeRegular = "Regular";
        private const string BlankValue = "[blank]";
        public const string Delimiter = ">";
        public const string NodeTypeDerived = "Derived";
        public const string CrossPrefix = "CL From -> ";
        private int _skuCount;

        #endregion Fields

        #region Properties (10)

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

        public List<TaxonomyInfo> AllChildren2
        {
            get
            {
                if (IsLeafNode)
                {
                    return
                        ChildTaxonomyDatas.Where(p => p.Active)
                            .Traverse(p => p.TaxonomyInfo.ChildTaxonomyDatas.Where(q => q.Active))
                            .Select(p => p.TaxonomyInfo)
                            .Union(this)
                            .ToList();
                }

                return
                    ChildTaxonomyDatas.Where(p => p.Active)
                        .Traverse(p => p.TaxonomyInfo.ChildTaxonomyDatas.Where(q => q.Active))
                        .Select(p => p.TaxonomyInfo)
                        .ToList();
            }
        }

        /// <summary>
        /// Gets all leaf children that have atleast one active sku, including the source if its a leaf.
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
                            .Where(p => p.TaxonomyInfo.SkuInfos.Any(q => q.Active))
                            .Select(p => p.TaxonomyInfo)
                            .Union(this);
                }

                return
                    ChildTaxonomyDatas.Where(p => p.Active)
                        .Traverse(p => p.TaxonomyInfo.ChildTaxonomyDatas.Where(q => q.Active))
                        .Where(p => p.TaxonomyInfo.SkuInfos.Any(q => q.Active))
                        .Select(p => p.TaxonomyInfo);
            }
        }

        public EntitySet<TaxonomyData> ChildTaxonomyDatas
        {
            get { return TaxonomyDatas1; }
            set { TaxonomyDatas1 = value; }
        }

        public int CurrentNodeLevel
        {
            get
            {
                if (TaxonomyData.ParentTaxonomyID == null)
                    return 1;

                return TaxonomyData.ParentTaxonomyInfo.CurrentNodeLevel + 1;
            }
        }

        /// <summary>
        /// Gets the get leaf taxnomies.
        /// This propery is very fast in comparison to the AllLeafChildren
        /// </summary>
        public IEnumerable<TaxonomyInfo> GetLeafTaxnomies
        {
            get
            {
                return
                    AryaTools.Instance.InstanceData.Dc.ExecuteQuery<TaxonomyInfo>(@"WITH Taxonomy1(NodeName,TaxId,ParentTaxId) AS (

	                                                                        SELECT      T1.NodeName, T1.TaxonomyID AS TaxId,T1.ParentTaxonomyID
	                                                                        FROM        dbo.TaxonomyInfo AS Ti 
				                                                                        INNER  JOIN
				                                                                        dbo.TaxonomyData AS T1 ON Ti.ID = T1.TaxonomyID AND T1.Active = 1
	                                                                        WHERE       T1.TaxonomyID  = {0}
	                                                                        UNION ALL
	                                                                        SELECT      T1.NodeName, T1.TaxonomyID AS TaxId,T1.ParentTaxonomyID
	                                                                        FROM        dbo.TaxonomyInfo AS Ti 
				                                                                        INNER JOIN dbo.TaxonomyData AS T1 ON Ti.ID = T1.TaxonomyID AND T1.Active = 1
				                                                                        INNER JOIN Taxonomy1 temp on temp.TaxId = T1.ParentTaxonomyID
	                                                                        )

	                                                                        SELECT ti.*
	                                                                        FROM Taxonomy1 t1 inner join TaxonomyInfo ti on t1.TaxId = ti.ID 
	                                                                        WHERE EXISTS 
	                                                                        (
		                                                                        SELECT *
		                                                                        FROM SkuInfo si
		                                                                        WHERE si.TaxonomyID = TaxId and si.Active = 1
	                                                                        )", ID.ToString());
            }
        }

        public bool IsLeafNode
        {
            get { return !ChildTaxonomyDatas.Any(tax => tax.Active); }
        }

        public bool HasSkus
        {
            get { return SkuInfos.Any(si => si.Active); }
        }

        public IEnumerable<Sku> Skus
        {
            get { return SkuInfos.Where(si => si.Active).Select(si => si.Sku); }
        }

        public string NodeName
        {
            get
            {
                var taxonomyData = TaxonomyData;
                return taxonomyData == null
                    ? BlankValue
                    : String.IsNullOrEmpty(taxonomyData.NodeName) ? BlankValue : taxonomyData.NodeName;
            }
        }

        public int SkuCount
        {
            get
            {
                if (_skuCount == 0)
                    _skuCount = SkuInfos.Count(si => si.Active);
                return _skuCount;
            }
        }

        public TaxonomyData TaxonomyData
        {
            get { return TaxonomyDatas.FirstOrDefault(td => td.Active); }
        }

        public IEnumerable<TaxonomyInfo> GetNodes(bool getChildren, bool getEmptyNodes)
        {
            if (!getChildren)
                return getEmptyNodes || HasSkus ? new List<TaxonomyInfo> {this} : new List<TaxonomyInfo>();

            return GetLeafTaxonomies(getEmptyNodes);
        }

        public IEnumerable<TaxonomyInfo> GetLeafTaxonomies(bool getEmptyNodes = false, bool includeItselfIfLeaf = true)
        {
            if (IsLeafNode)
            {
                if (!getEmptyNodes && !HasSkus)
                    return new List<TaxonomyInfo>(0);

                return new List<TaxonomyInfo> { this };
            }

            if (getEmptyNodes)
            {
                return
                    AryaTools.Instance.InstanceData.Dc.ExecuteQuery<TaxonomyInfo>(@"WITH Taxonomy1(TaxonomyID) AS (
                        SELECT      T1.TaxonomyID
                        FROM        dbo.TaxonomyData AS T1
                        WHERE       T1.TaxonomyID  = {0} AND T1.Active = 1
                        UNION ALL
                        SELECT      T1.TaxonomyID
                        FROM        dbo.TaxonomyData AS T1
                                    INNER JOIN Taxonomy1 temp ON temp.TaxonomyID = T1.ParentTaxonomyID AND T1.Active = 1
                        )
                        SELECT ti.*
                        FROM Taxonomy1 t1 
                        INNER JOIN TaxonomyInfo ti ON t1.TaxonomyID = ti.ID", ID.ToString());
            }

            return AryaTools.Instance.InstanceData.Dc.ExecuteQuery<TaxonomyInfo>(@"WITH Taxonomy1(TaxonomyID) AS (
                    SELECT      T1.TaxonomyID
                    FROM        dbo.TaxonomyData AS T1
                    WHERE       T1.TaxonomyID  = {0} AND T1.Active = 1
                    UNION ALL
                    SELECT      T1.TaxonomyID
                    FROM        dbo.TaxonomyData AS T1
                                INNER JOIN Taxonomy1 temp ON temp.TaxonomyID = T1.ParentTaxonomyID AND T1.Active = 1
                    )
                    SELECT ti.*
                    FROM Taxonomy1 t1 
                    INNER JOIN TaxonomyInfo ti ON t1.TaxonomyID = ti.ID 
                    WHERE EXISTS 
                    (
                        SELECT *
                        FROM SkuInfo si
                        WHERE si.TaxonomyID = ti.ID AND si.Active = 1
                    )", ID.ToString());
        }

        #endregion Properties

        #region Methods (15)

        // Public Methods (12) 

        public TaxonomyData OldTaxonomyData
        {
            get { return TaxonomyDatas.OrderBy(td => td.CreatedOn).FirstOrDefault(); }
        }

        public TaxonomyInfo GetAncestorNode(int level)
        {
            if (level == 0)
                return null;

            if (CurrentNodeLevel > level)
                return TaxonomyData.ParentTaxonomyInfo.GetAncestorNode(level);

            return this;
        }

        public static List<Attribute> GetCalculatedAttributes(TaxonomyInfo taxonomy)
        {
            List<Attribute> calculatedAttributes;

            if (taxonomy == null || taxonomy.TaxonomyData == null)
            {
                calculatedAttributes = (from att in AryaTools.Instance.InstanceData.Dc.DerivedAttributes
                                        where
                                            att.TaxonomyInfo == null
                                            && att.Attribute.Project.Equals(AryaTools.Instance.InstanceData.CurrentProject)
                                        select att.Attribute).ToList();
            }
            else
            {
                calculatedAttributes = (from att in AryaTools.Instance.InstanceData.Dc.DerivedAttributes
                                        where
                                            att.TaxonomyInfo.Equals(taxonomy)
                                            && att.Attribute.ProjectID == AryaTools.Instance.InstanceData.CurrentProject.ID
                                        select att.Attribute).ToList();

                var parentAttributes =
                    GetCalculatedAttributes(taxonomy.TaxonomyData.ParentTaxonomyInfo)
                        .Where(att => !calculatedAttributes.Select(ca => ca.ID).Contains(att.ID));
                calculatedAttributes.AddRange(parentAttributes);
            }

            return calculatedAttributes;
        }

        public static List<Attribute> GetCalculatedAttributes(Guid taxonomyID)
        {
            var attrList = (from att in AryaTools.Instance.InstanceData.Dc.DerivedAttributes
                            where
                                att.TaxonomyID == taxonomyID
                                && att.Attribute.ProjectID == AryaTools.Instance.InstanceData.CurrentProject.ID
                            select att.Attribute).ToList();

            return attrList;
        }

        private static List<List<T>> SplitList<T>(List<T> parentList, int size)
        {
            var listOfList = new List<List<T>>();
            for (var i = 0; i < parentList.Count; i += size)
                listOfList.Add(parentList.Skip(i).Take(size).ToList());
            return listOfList;
        }

        public static IEnumerable<TaxonomyInfo> GetLevel1Nodes()
        {
            var level1Nodes = from td in AryaTools.Instance.InstanceData.Dc.TaxonomyDatas
                              where
                                  td.Active && td.TaxonomyInfo.ProjectID.Equals(AryaTools.Instance.InstanceData.CurrentProject.ID)
                                  && td.ParentTaxonomyID == null
                              select td.TaxonomyInfo;

            return level1Nodes;
            //AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.Where(
            //    ti =>
            //    ti.ProjectID.Equals(AryaTools.Instance._currentProject.ID) &&
            //    ti.TaxonomyDatas.Where(td => td.Active).First().ParentTaxonomyID == null);
        }

        public static int GetNodeCount(TaxonomyInfo taxonomy)
        {
            IEnumerable<TaxonomyInfo> childTaxonomyInfos;
            var count = 0;
            if (taxonomy != null)
            {
                childTaxonomyInfos =
                    taxonomy.ChildTaxonomyDatas.Where(td => td.Active).Select(td => td.TaxonomyInfo).ToList();
                count = childTaxonomyInfos.Count();
            }
            else
                childTaxonomyInfos = GetLevel1Nodes().ToList();

            count += childTaxonomyInfos.Sum(childNode => GetNodeCount(childNode));

            return count;
        }

        public void InsertDisplayOrder(Attribute attribute, decimal displayOrder, bool reorderAttributes)
        {
            var currentSchemaInfo = SchemaInfos.FirstOrDefault(si => si.Attribute.ID == attribute.ID);

            if (currentSchemaInfo != null && currentSchemaInfo.SchemaData != null
                && currentSchemaInfo.SchemaData.DisplayOrder == displayOrder)
                return;

            if (displayOrder != 0)
            {
                var existingValues =
                    SchemaInfos.Where(si => si.Attribute.Equals(attribute) && si.SchemaData != null)
                        .Select(si => si.SchemaData)
                        .Where(sd => sd.DisplayOrder == displayOrder)
                        .Select(sd => sd.SchemaInfo.Attribute)
                        .ToList();

                existingValues.ForEach(att => InsertDisplayOrder(att, displayOrder + 1, false));
            }

            if (currentSchemaInfo == null)
                currentSchemaInfo = new SchemaInfo { Attribute = attribute, TaxonomyInfo = this };

            if (currentSchemaInfo.SchemaData == null)
            {
                currentSchemaInfo.SchemaDatas.Add(new SchemaData
                                                  {
                                                      SchemaID = currentSchemaInfo.ID,
                                                      DisplayOrder = displayOrder,
                                                      InSchema = true
                                                  });
            }
            else
            {
                var schemaDataToBeDeleted = currentSchemaInfo.SchemaData;
                schemaDataToBeDeleted.DeletedOn = DateTime.Now;
                //schemaDataToBeDeleted.DeletedBy = AryaTools.Instance.InstanceData.CurrentUser.ID;
                schemaDataToBeDeleted.DeletedByUser = AryaTools.Instance.InstanceData.CurrentUser;
                //fixes Object not found error
                schemaDataToBeDeleted.Active = false;

                var newSchemaData = new SchemaData
                                    {
                                        SchemaInfo = currentSchemaInfo,
                                        DisplayOrder = displayOrder,
                                        NavigationOrder = schemaDataToBeDeleted.NavigationOrder,
                                        InSchema = schemaDataToBeDeleted.InSchema,
                                        Active = true
                                    };

                currentSchemaInfo.SchemaDatas.Add(newSchemaData);
            }
        }

        public void InsertNavigationOrder(Attribute attribute, decimal navigationOrder, bool reorderAttributes)
        {
            var currentSchemaInfo = SchemaInfos.FirstOrDefault(si => si.Attribute.ID == attribute.ID);

            if (currentSchemaInfo != null && currentSchemaInfo.SchemaData != null
                && currentSchemaInfo.SchemaData.NavigationOrder == navigationOrder)
                return;

            if (navigationOrder != 0)
            {
                var existingValues =
                    SchemaInfos.Where(si => si.Attribute.Equals(attribute) && si.SchemaData != null)
                        .Select(si => si.SchemaData)
                        .Where(sd => sd.DisplayOrder == navigationOrder)
                        .Select(sd => sd.SchemaInfo.Attribute)
                        .ToList();

                existingValues.ForEach(att => InsertNavigationOrder(att, navigationOrder + 1, false));
            }

            if (currentSchemaInfo == null)
                currentSchemaInfo = new SchemaInfo { Attribute = attribute, TaxonomyInfo = this };

            if (currentSchemaInfo.SchemaData == null)
            {
                currentSchemaInfo.SchemaDatas.Add(new SchemaData
                                                  {
                                                      SchemaID = currentSchemaInfo.ID,
                                                      NavigationOrder = navigationOrder,
                                                      InSchema = true
                                                  });
            }
            else
            {
                var schemaDataToBeDeleted = currentSchemaInfo.SchemaData;
                schemaDataToBeDeleted.DeletedOn = DateTime.Now;
                //schemaDataToBeDeleted.DeletedBy = AryaTools.Instance.InstanceData.CurrentUser.ID;
                schemaDataToBeDeleted.DeletedByUser = AryaTools.Instance.InstanceData.CurrentUser;
                //fixes Object not found error
                schemaDataToBeDeleted.Active = false;

                var newSchemaData = new SchemaData
                                    {
                                        SchemaInfo = currentSchemaInfo,
                                        NavigationOrder = navigationOrder,
                                        DisplayOrder = schemaDataToBeDeleted.DisplayOrder,
                                        InSchema = schemaDataToBeDeleted.InSchema,
                                        Active = true
                                    };

                currentSchemaInfo.SchemaDatas.Add(newSchemaData);
            }
        }

        public bool IsChildOf(TaxonomyInfo nodeToTest)
        {
            return TaxonomyData.ParentTaxonomyInfo != null
                   && (TaxonomyData.ParentTaxonomyInfo == nodeToTest
                       || TaxonomyData.ParentTaxonomyInfo.IsChildOf(nodeToTest));
        }

        public string OldString()
        {
            return OldTaxonomyData == null
                ? String.Empty
                : (OldTaxonomyData.ParentTaxonomyID == null
                    ? OldTaxonomyData.NodeName
                    : String.Format("{1}{0}{2}", Delimiter, OldTaxonomyData.ParentTaxonomyInfo.OldString(),
                        OldTaxonomyData.NodeName));
        }

        //public static void SplitTaxonomyString(string taxString, out string level1String, out string level2String,
        //    out string level3String)
        //{
        //    var level1Index = taxString.IndexOf(Delimiter, StringComparison.Ordinal);
        //    if (level1Index == -1)
        //        level1String = level2String = level3String = taxString;
        //    else
        //    {
                
        //        level1String = taxString.Substring(0, level1Index);
        //        var level2Index = taxString.Length > level1Index
        //            ? taxString.IndexOf(Delimiter, level1Index + 1, StringComparison.Ordinal)
        //            : -1;
        //        if (level2Index == -1)
        //            level2String = level3String = taxString;
        //        else
        //        {
        //            level2String = taxString.Substring(0, level2Index);
        //            var level3Index = taxString.Length > level2Index
        //                ? taxString.IndexOf(Delimiter, level2Index + 1, StringComparison.Ordinal)
        //                : -1;
        //            if (level3Index == -1)
        //                level3String = taxString;
        //            else
        //                level3String = taxString.Substring(0, level3Index);
        //        }
        //    }
        //}


       
        public static string StripTopLevelNodes(string fullTaxonomy, int numberOfLevelsToStrip)
        {
            var result = fullTaxonomy;

            for (var i = 0; i < numberOfLevelsToStrip; i++)
            {
                var index = result.IndexOf(Delimiter, StringComparison.Ordinal);
                result = result.Length > index ? result.Substring(index + 1) : String.Empty;
            }

            return result;
        }

        public override string ToString()
        {
            var taxonomyData = TaxonomyData;
            return taxonomyData == null
                ? String.Empty
                : (taxonomyData.ParentTaxonomyInfo == null
                    ? taxonomyData.NodeName
                    : String.Format("{1}{0}{2}", Delimiter, taxonomyData.ParentTaxonomyInfo,
                        String.IsNullOrEmpty(taxonomyData.NodeName) ? BlankValue : taxonomyData.NodeName));
        }

        // Private Methods (3) 

        partial void OnCreated() { SkuDataDbDataContext.DefaultTableValues(this); }

        #endregion Methods

        public Framework.Data.AryaDb.Sku EnrichmentImageSku
        {
            get
            {
                string messageImage;
                try
                {
                    var enrichmentImage = String.Empty;
                    var tmi =
                        TaxonomyMetaInfos.FirstOrDefault(
                            t => t.Attribute.AttributeName == Resources.TaxonomyEnrichmentImageAttributeName);
                    if (tmi != null)
                    {
                        var tmd = tmi.TaxonomyMetaDatas.FirstOrDefault(t => t.Active);
                        if (tmd != null)
                            enrichmentImage = tmd.Value;
                    }

                    messageImage = enrichmentImage;
                }
                catch (Exception)
                {
                    messageImage = string.Empty;
                }

                using (
                    var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                        AryaTools.Instance.InstanceData.CurrentUser.ID))
                {
                    return (from si in dc.SkuInfos
                        where si.Active && si.TaxonomyID == ID && si.Sku.ItemID == messageImage
                        select si.Sku).FirstOrDefault();
                }
            }
        }

        public string EnrichmentImage
        {
            get
            {
                if (EnrichmentImageSku == null)
                    return string.Empty;

                using (
                    var dc = new AryaDbDataContext(AryaTools.Instance.InstanceData.CurrentProject.ID,
                        AryaTools.Instance.InstanceData.CurrentUser.ID))
                {
                    var imageSku = dc.Skus.First(sku => sku.ID == EnrichmentImageSku.ID);
                    var currentImageManager = new ImageManager(dc, AryaTools.Instance.InstanceData.CurrentProject.ID)
                                              {
                                                  ImageSku
                                                      =
                                                      imageSku
                                              };
                    return Path.GetFileName(currentImageManager.OriginalFileUri);
                }
            }
        }

        public string EnrichmentCopy
        {
            get
            {
                string messageCopy;
                try
                {
                    var enrichmentCopy = String.Empty;
                    var tmi =
                        TaxonomyMetaInfos.FirstOrDefault(
                            t => t.Attribute.AttributeName == Resources.TaxonomyEnrichmentCopyAttributeName);
                    if (tmi != null)
                    {
                        var tmd = tmi.TaxonomyMetaDatas.FirstOrDefault(t => t.Active);
                        if (tmd != null)
                            enrichmentCopy = tmd.Value;
                    }

                    messageCopy = enrichmentCopy;
                }
                catch (Exception)
                {
                    messageCopy = string.Empty;
                }
                return messageCopy;
            }
        }

        #region IComparable Members

        public int CompareTo(object other) { return String.CompareOrdinal(ToString(), other.ToString()); }

        #endregion

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
            }
        }

        internal IEnumerable<string> RankedAttributes()
        {
            return from si in SchemaInfos
                   let sd = si.SchemaData
                   where sd != null && (sd.NavigationOrder > 0 || sd.DisplayOrder > 0)
                   select si.Attribute.AttributeName;
        }

        public static List<string> GetTaxonomyLevels(string taxString, int numberOfTaxLevelItem = 4)
        {
            var outTaxLevels = new string[numberOfTaxLevelItem];
            int nodeCount = taxString.Split(Delimiter.ToArray()).Length - 2;//-1 for 0 base index, -1 for leaf node ignore
           
            string[] currentTaxLevels = taxString.Split(Delimiter.ToArray());
            while (numberOfTaxLevelItem > 0 )
            {
                if (nodeCount < 0)
                    nodeCount = 0;
                string taxNodelevelPrefix = string.Format("[L{0}]", nodeCount +1);
                outTaxLevels[numberOfTaxLevelItem - 1] = taxNodelevelPrefix + currentTaxLevels[nodeCount];
                numberOfTaxLevelItem--;
                nodeCount --;
            }
            
            return outTaxLevels.ToList();
        }

        public static List<string> GetTaxonomyLevelFullPaths(string taxString, int numberOfTaxLevelItem  = 4)
        {
            var outTaxLevelFullPaths = new string[numberOfTaxLevelItem];
            var currentLevelIndex = taxString.LastIndexOf(Delimiter.ToArray().First()) - 1;//ignore leaf node
            while (numberOfTaxLevelItem > 0)
            {
                if (currentLevelIndex < 0)
                {
                    outTaxLevelFullPaths[numberOfTaxLevelItem - 1] = taxString;
                    numberOfTaxLevelItem--;
                    continue;
                }
                outTaxLevelFullPaths[numberOfTaxLevelItem - 1] = taxString.Substring(0, currentLevelIndex);
                taxString = outTaxLevelFullPaths[numberOfTaxLevelItem - 1];
                currentLevelIndex = taxString.LastIndexOf(Delimiter.ToArray().First());
                numberOfTaxLevelItem--;
            }

            return outTaxLevelFullPaths.ToList();
        }

        string ISpell.GetType()
        {
            return this.GetType().ToString(); 
        }

        Dictionary<string, string> ISpell.GetSpellValue()
        {
            var _propertyNameValue = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(this.TaxonomyData.NodeDescription))
            {
                _propertyNameValue.Add("Node Description", this.TaxonomyData.NodeDescription);
            }
            if (!String.IsNullOrEmpty(this.TaxonomyData.NodeName))
            {
                _propertyNameValue.Add("Nodename", this.TaxonomyData.NodeName);
            }

            return _propertyNameValue;
        }
        private KeyValuePair<bool, string>? _updatable = new KeyValuePair<bool, string>(true, "");
        KeyValuePair<bool, string> ISpell.Updatable
        {
            get
            {
                return (KeyValuePair<bool, string>)_updatable; ;
            }
        }

        Guid ISpell.GetId()
        {
            return this.ID;
        }

        ISpell ISpell.SetValue(string propertyName, string value)
        {
            TaxonomyData taxData = null;
            if (propertyName.ToLower() == "nodename")
            {
                taxData = new TaxonomyData()
                {
                    NodeName = value,
                    NodeDescription = this.TaxonomyData.NodeDescription,
                    ParentTaxonomyInfo = this.TaxonomyData.ParentTaxonomyInfo,
                    TaxonomyInfo = this
                };
            }
            else if (propertyName.ToLower() == "node description")
            {
                taxData = new TaxonomyData()
                {
                    NodeName = this.TaxonomyData.NodeName,
                    NodeDescription = value,
                    ParentTaxonomyInfo = this.TaxonomyData.ParentTaxonomyInfo,
                    TaxonomyInfo = this
                };
            }
            if (taxData != null)
            {
                this.TaxonomyData.Active = false;
                AryaTools.Instance.InstanceData.Dc.TaxonomyDatas.InsertOnSubmit(taxData);
            }
            return this;
        }

        string ISpell.GetLocation()
        {
            return "";// this.TaxonomyData.ToString();
        }
    }
}