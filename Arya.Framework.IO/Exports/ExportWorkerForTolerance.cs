using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ComponentModel;
using Arya.Framework.Common.ComponentModel;
using Arya.Framework.Common.Extensions;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO.InterchangeRecords;
using Attribute = Arya.Framework.Data.AryaDb.Attribute;
using Arya.Framework.Utility;

namespace Arya.Framework.IO.Exports
{
    [Serializable]
    [DisplayName(@"Tolerance")]
    public class ExportWorkerForTolerance : ExportWorkerBase
    {
        #region Fields

        private readonly DataTable _toleranceTable = new DataTable("Tolerance")
        {
            Columns =
            {
                {"Node Item Id", typeof (String)},
                {"Matched Item Id", typeof (String)}
            }
        };
        private CombinedInterchangeData _data;

        private ToleranceExportArgs _args;
        private const string Tolerance = "Tolerance";

        #endregion

        #region Constructor

        public ExportWorkerForTolerance(string argumentFilePath)
            : base(argumentFilePath, typeof (ToleranceExportArgs))
        {
        }

        #endregion

        #region Public Methods

        public virtual bool IsInputValid()
        {
            throw new NotImplementedException();
        }

        protected override void FetchExportData()
        {
            _args = (ToleranceExportArgs) Arguments;
            if (_args.ExportNif)
            {
                _data = new CombinedInterchangeData(true);
            }

            var exportTaxonomyIds = _args.TaxonomyIds;
            var exportTaxonomies = CurrentDb.TaxonomyInfos.Where(p => exportTaxonomyIds.Contains(p.ID)).ToList();
            var allExportTaxonomies = exportTaxonomies.SelectMany(p => p.AllLeafChildren).Distinct().ToList();

            foreach (var taxonomy in allExportTaxonomies)
            {
                ProcessTaxonomy(taxonomy);
            }

            ExportDataTables.Add(_toleranceTable);
        }

        protected override void SaveExportData()
        {
            // run superclass code
            base.SaveExportData();

            // save Interchange Format file
            if (_args.ExportNif)
            {
                _data.DedupLists();
                using (TextWriter file = new StreamWriter(Path.Combine(ArgumentDirectoryPath, _args.BaseFilename + "_AryaInterchangeFormat.xml")))
                {
                    _data.SerializeObject(file);
                }
            }
        }

        #endregion

        #region Private Methods

        private void ProcessTaxonomy(TaxonomyInfo taxonomy)
        {
            // get attributes for this taxonomy
            var toleranceTaxAttributes = 
                (from si in taxonomy.SchemaInfos
                    let opt = (from smi in si.SchemaMetaInfos
                        where smi.Attribute.AttributeName.Equals("Is Optional")
                        from smd in smi.SchemaMetaDatas
                        where smd.Active
                        select smd.Value).FirstOrDefault()
                    let att = si.Attribute
                    let tol = 
                        (from smi in si.SchemaMetaInfos
                             where smi.Attribute.AttributeName.Equals(Tolerance)
                             from smd in smi.SchemaMetaDatas
                                where smd.Active
                             select smd.Value).FirstOrDefault()
                    let isOptional = opt != null && (opt.Equals("Yes"))
                    select new ToleranceAttribute(att, tol, isOptional)).Where(t => !String.IsNullOrEmpty(t.Tolerance)).ToList();

            if (!toleranceTaxAttributes.Any())
            {
                return;
            }

            IQueryable<Sku> skuQuery = CurrentDb.Skus;
            foreach (var ta in toleranceTaxAttributes.Where(b => !b.IsOptional))
            {
                var attName = ta.Attr.AttributeName;
                skuQuery =
                    skuQuery.Where(
                        ei =>
                            ei.EntityInfos.Any(
                                e => e.EntityDatas.Any(ed => ed.Attribute.AttributeName == attName && ed.Active)));

            }

            var filteredSkus = skuQuery.Where(si => si.SkuInfos.FirstOrDefault(a => a.Active).TaxonomyID == taxonomy.ID).ToList();
            var nodeSkus = taxonomy.SkuInfos.Where(s => s.Active).Select(s => s.Sku);

            var matchedSkus = new Dictionary<Sku, List<Sku>>();
            foreach (var sku in nodeSkus)
            {
                if (!filteredSkus.Contains(sku))
                {
                    continue;
                }
                AddToleratedSkus(toleranceTaxAttributes, filteredSkus, matchedSkus, sku);
            }

            WriteToleranceRows(matchedSkus);
            if (_args.ExportNif)
            {
                WriteInterchangeRecords(matchedSkus);
            }
        }

        private void AddToleratedSkus(List<ToleranceAttribute> toleranceTaxAttributes, List<Sku> filteredSkus, Dictionary<Sku, List<Sku>> matchedSkus, Sku sku)
        {
            var toleratedSkus = new List<Sku>();
            foreach (var filteredSku in filteredSkus)
            {
                bool hit = false;
                if (filteredSku == sku)
                {
                    continue;
                }

                foreach (var ta in toleranceTaxAttributes)
                {
                    var tolerence = ta.Tolerance.Trim().ToLower();
                    if (String.IsNullOrEmpty(tolerence))
                    {
                        continue;
                    }
                    hit = ToleranceMatch(sku, filteredSku, ta.Attr.AttributeName, tolerence, ta.IsOptional);
                    if (!hit)
                    {
                        break;
                    }
                }
                if (hit)
                {
                    toleratedSkus.Add(filteredSku);
                }
            }

            if (toleratedSkus.Any())
            {
                matchedSkus.Add(sku, toleratedSkus);
            }
        }

        private bool ToleranceMatch(Sku nodeSku, Sku checkSku, string attributeName, string rawTolerence, bool isOptional)
        {
            var baseUnitConversion = new BaseUnitConversion(CurrentDb);
            var nodeSkuEds = nodeSku.GetValuesForAttribute(CurrentDb, attributeName).ToList();
            var checkSkuEds = checkSku.GetValuesForAttribute(CurrentDb, attributeName).ToList();

            if ((!nodeSkuEds.Any() || !checkSkuEds.Any()) && isOptional)
                return true;

            if (rawTolerence.Equals("="))
            {
                var nodeSkuValues = nodeSkuEds.Select(v => v.Value.ToLower()).ToArray();
                var checkSkuValues = checkSkuEds.Select(v => v.Value.ToLower()).ToArray();
                return (nodeSkuValues.Intersect(checkSkuValues).Any());
            }

            if (rawTolerence.Contains('='))
            {
                var nodeSkuValues = nodeSkuEds.Select(v => v.Value.ToLower()).ToArray();
                var checkSkuValues = checkSkuEds.Select(v => v.Value.ToLower()).ToArray();
                return IsLovTolerant(rawTolerence, nodeSkuValues, checkSkuValues);
            }
            if (rawTolerence.StartsWith("+") || rawTolerence.StartsWith("-") || rawTolerence.StartsWith("~") || rawTolerence.StartsWith("0"))
            {
                var nodeSkuNumericValues = nodeSkuEds.Select(v => baseUnitConversion.GetBaseValue(v.Value, v.Uom)).ToList();
                var checkSkuNumericValues = checkSkuEds.Select(v => baseUnitConversion.GetBaseValue(v.Value, v.Uom)).ToList();

                if (nodeSkuNumericValues.Contains(int.MaxValue))
                    nodeSkuNumericValues.Remove(int.MaxValue);
                if (checkSkuNumericValues.Contains(int.MaxValue))
                    checkSkuNumericValues.Remove(int.MaxValue);

                return nodeSkuNumericValues.Any(val => baseUnitConversion.IsTolerated(checkSkuNumericValues, val, rawTolerence));
            }
            if (!string.IsNullOrEmpty(rawTolerence))
            {
                var nodeSkuValues = nodeSkuEds.Select(v => v.Value.ToLower()).ToArray();
                var checkSkuValues = checkSkuEds.Select(v => v.Value.ToLower()).ToArray();
                return (nodeSkuValues.Contains(rawTolerence) & checkSkuValues.Contains(rawTolerence));
            }
            return false;
        }

        private static bool IsLovTolerant(string tolerence, string[] nodeSkuvalues, string[] checkSkuValues)
        {
            var lovs = tolerence.ToLower().Split('|').Select(p => p.Trim()).ToArray();
            foreach (var lov in lovs)
            {
                var validValues = lov.Split('=').Select(p => p.Trim()).ToArray();
                IEnumerable<string> lovNodeSkuValues = validValues.Intersect(nodeSkuvalues);
                IEnumerable<string> lovCheckSkuValues = validValues.Intersect(checkSkuValues);

                if (lovCheckSkuValues.Any() && lovNodeSkuValues.Any())
                    return true;

                if (nodeSkuvalues.Intersect(checkSkuValues).Any())
                {
                    return true;
                }
            }
            return false;
        }

        private void WriteToleranceRows(Dictionary<Sku, List<Sku>> matchedSkus)
        {
            foreach (var matchedSku in matchedSkus)
            {
                foreach (var sku in matchedSku.Value)
                {
                    DataRow newRow = _toleranceTable.NewRow();
                    newRow["Node Item Id"] = matchedSku.Key.ItemID;
                    newRow["Matched Item Id"] = sku.ItemID;

                    _toleranceTable.Rows.Add(newRow);
                }
            }
        }

        private void WriteInterchangeRecords(Dictionary<Sku, List<Sku>> matchedSkus)
        {
            foreach (var matchedSku in matchedSkus)
            {
                foreach (var sku in matchedSku.Value)
                {
                    _data.SkuLinks.Add(new SkuLinkInterchangeRecord
                    {
                        FromItemID = matchedSku.Key.ItemID,
                        LinkType = Tolerance,
                        ToItemID = sku.ItemID
                    });
                }
            }
        }

        #endregion
    }

    [Serializable]
    public class ToleranceAttribute
    {
        #region Properties

        public Attribute Attr { get; set; }
        public string Tolerance { get; set; }
        public bool IsOptional { get; set; }

        #endregion

        #region Constructor

        public ToleranceAttribute(Attribute attr, string tol, bool isOpt)
        {
            Attr = attr;
            Tolerance = tol;
            IsOptional = isOpt;
        }

        #endregion
    }

    [Serializable]
    public class ToleranceExportArgs : ExportArgs
    {
        #region Properties

        [DefaultValue(false)]
        [Category(CaptionOptional)]
        [PropertyOrder(OptionalBaseOrder)]
        [Description(" If YES, the export will additionally produce a Arya Interchange Format file with the Tolerance data. If NO, only the selected file format will be produced.")]
        [DisplayName(@"Export Arya Interchange Format file")]
        [TypeConverter(typeof(BooleanToYesNoConverter))]
        public bool ExportNif { get; set; }

        #endregion

        #region Constructor

        public ToleranceExportArgs()
        {
            HiddenProperties += "ExportCrossListNodes" + "IgnoreT1Taxonomy";
        }

        #endregion
    }
}
