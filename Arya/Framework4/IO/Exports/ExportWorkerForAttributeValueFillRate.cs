using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework.Common;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Extensions;
using Arya.Framework4.ComponentModel;
using Arya.HelperClasses;

namespace Arya.Framework4.IO.Exports
{
    public class ExportWorkerForAttributeValueFillRate : ExportWorker
    {
        private TextWriter avfrFile;
        private string fieldDelimiter;

        public ExportWorkerForAttributeValueFillRate(string argumentDirectoryPath, PropertyGrid ownerPropertyGrid)
            : base(argumentDirectoryPath, ownerPropertyGrid)
        {
            ownerPropertyGrid.SelectedObject = this;
            AllowMultipleTaxonomySelection = true;
            WorkerSupportsSaveOptions = false;
        }

        public override void Run()
        {
            State = WorkerState.Working;
            StatusMessage = "Init";
            fieldDelimiter = FieldDelimiter.GetValue().ToString();

            var fi = new FileInfo(ExportFileName);
            var baseFileName = fi.FullName.Replace(fi.Extension, string.Empty);
            avfrFile = new StreamWriter(baseFileName + "_AVFR.txt", false, Encoding.UTF8);

            StatusMessage = "Generating Child node list ... ";

            var allChildren =
                Taxonomies.Cast<ExtendedTaxonomyInfo>().SelectMany(p => p.Taxonomy.AllLeafChildren).Distinct().ToList();

            CurrentProgress = 0;
            MaximumProgress = allChildren.Count;

            avfrFile.WriteLine(
                "T1{0}T2{0}T3{0}T4{0}T5{0}T6{0}T7{0}AttributeName{0}Value{0}NodeSkuCount{0}ValueSkuCount{0}AttributeSkuCount{0}NavigationOrder{0}DisplayOrder{0}Attribute FillRate{0}Value FillRate",
                fieldDelimiter);

            foreach (var taxonomyInfo in allChildren)
            {
                WriteAVFRData(taxonomyInfo);
                CurrentProgress++;
            }

            avfrFile.Close();
            avfrFile.Dispose();

            StatusMessage = "Done.";
            State = WorkerState.Ready;
        }

        public virtual bool IsInputValid() { throw new NotImplementedException(); }

        private void WriteAVFRData(TaxonomyInfo taxonomy)
        {
            StatusMessage = "Generating AVFR Data - NonBlanks";

            var gRPSData = AryaTools.Instance.InstanceData.Dc.ExecuteQuery<GRPS>(@"
                                                                        WITH Taxonomy1(NodeName,TaxId,ParentTaxId) AS (
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
                                                                    ),
                                                                    LeafNodes(TaxID) AS 
                                                                    (
	                                                                    SELECT t1.TaxId
	                                                                    FROM Taxonomy1 t1
	                                                                    WHERE EXISTS 
	                                                                    (
		                                                                    SELECT *
		                                                                    FROM SkuInfo si
		                                                                    WHERE si.TaxonomyID = TaxId and si.Active = 1
	                                                                    )
                                                                    )

                                                                        select TaxId,AttributeName,Value,COUNT(distinct ItemID) as ValueSkuCount,
									                                                                         (
										                                                                        select COUNT(*)
										                                                                        from SkuInfo si 
										                                                                        where si.TaxonomyID = ssd.TaxId
										                                                                        and si.Active = 1
									                                                                         ) as NodeSkuCount,NavigationOrder,DisplayOrder
								     
								     
                                                                        from SkuSchemaData ssd
                                                                        where TaxId IN ( Select * from LeafNodes)
                                                                        and ssd.InSchema = 1
                                                                        Group by GROUPING SETS((TaxId,AttributeName,NavigationOrder,DisplayOrder,Value),(TaxId,AttributeName,NavigationOrder,DisplayOrder))",
                taxonomy.ID).ToList();

            StatusMessage = "Generating AVFR Data - Blanks";

            var blanks = (from grp in gRPSData
                where grp.Value == null && (grp.NodeSkuCount - grp.ValueSkuCount) > 0
                select
                    new
                    {
                        grp.TaxID,
                        grp.AttributeName,
                        Value = string.Empty,
                        grp.NodeSkuCount,
                        ValueSkuCount = grp.NodeSkuCount - grp.ValueSkuCount,
                        AttributeSkuCount = grp.ValueSkuCount,
                        grp.NavigationOrder,
                        grp.DisplayOrder
                    });

            var nonBlanks = (from grp1 in gRPSData
                join grp2 in gRPSData on new {grp1.TaxID, grp1.AttributeName} equals
                    new {grp2.TaxID, grp2.AttributeName}
                where grp1.Value != null && grp2.Value == null
                select
                    new
                    {
                        grp1.TaxID,
                        grp1.AttributeName,
                        grp1.Value,
                        grp1.NodeSkuCount,
                        grp1.ValueSkuCount,
                        AttributeSkuCount = grp2.ValueSkuCount,
                        grp1.NavigationOrder,
                        grp1.DisplayOrder
                    });

            var finalData = nonBlanks.Union(blanks).ToList();

            StatusMessage = "Featching Taxonomies";

            var taxonomyParts =
                AryaTools.Instance.InstanceData.Dc.ExecuteQuery<TaxonomyPart>(@"
                                                        WITH Taxonomy1(NodeName,TaxId,ParentTaxId) AS (

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

                                                        SELECT t.TaxId,T1,T2,T3,T4,T5,T6,T7
                                                        FROM Taxonomy1 t1 inner join Taxonomy t on t1.TaxId = t.TaxId 
                                                        WHERE t.IsTerminal =1", taxonomy.ID)
                    .ToDictionary(key => key.TaxID, value => value);

            StatusMessage = "Writing Output file ... ";

            foreach (var data in finalData)
            {
                var taxonomyPart = taxonomyParts[data.TaxID];
                avfrFile.WriteLine(
                    "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}",
                    fieldDelimiter, taxonomyPart.T1, taxonomyPart.T2, taxonomyPart.T3, taxonomyPart.T4, taxonomyPart.T5,
                    taxonomyPart.T6, taxonomyPart.T7, data.AttributeName, data.Value, data.NodeSkuCount,
                    data.ValueSkuCount, data.AttributeSkuCount, data.NavigationOrder, data.DisplayOrder,
                    Math.Round((decimal) data.AttributeSkuCount/data.NodeSkuCount, 2),
                    Math.Round((decimal) data.ValueSkuCount/data.NodeSkuCount, 2));
            }
        }

        #region Nested type: GRPS

        internal class GRPS
        {
            public string AttributeName;
            public decimal DisplayOrder;
            public decimal NavigationOrder;
            public int NodeSkuCount;
            public Guid TaxID;
            public string Value;
            public int ValueSkuCount;
        }

        #endregion

        #region Nested type: TaxonomyPart

        internal class TaxonomyPart
        {
            public Guid TaxID;
            private string t1;
            private string t2;
            private string t3;
            private string t4;
            private string t5;
            private string t6;
            private string t7;

            public string T1
            {
                get { return string.IsNullOrWhiteSpace(t1) ? string.Empty : t1; }
                set { t1 = value; }
            }

            public string T2
            {
                get { return string.IsNullOrWhiteSpace(t2) ? string.Empty : t2; }
                set { t2 = value; }
            }

            public string T3
            {
                get { return string.IsNullOrWhiteSpace(t3) ? string.Empty : t3; }
                set { t3 = value; }
            }

            public string T4
            {
                get { return string.IsNullOrWhiteSpace(t4) ? string.Empty : t4; }
                set { t4 = value; }
            }

            public string T5
            {
                get { return string.IsNullOrWhiteSpace(t5) ? string.Empty : t5; }
                set { t5 = value; }
            }

            public string T6
            {
                get { return string.IsNullOrWhiteSpace(t6) ? string.Empty : t6; }
                set { t6 = value; }
            }

            public string T7
            {
                get { return string.IsNullOrWhiteSpace(t7) ? string.Empty : t7; }
                set { t7 = value; }
            }
        }

        #endregion
    }
}