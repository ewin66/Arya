using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LinqKit;
using Arya.Framework.Common;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.Utility;
using Arya.HelperClasses;
using Arya.Properties;
using Attribute = Arya.Data.Attribute;
using EntityData = Arya.Data.EntityData;
using EntityInfo = Arya.Data.EntityInfo;
using Project = Arya.Data.Project;
using Sku = Arya.Data.Sku;
using SkuInfo = Arya.Data.SkuInfo;
using TaxonomyData = Arya.Data.TaxonomyData;
using TaxonomyInfo = Arya.Data.TaxonomyInfo;
using UserProject = Arya.Data.UserProject;

//Patch Import for Attributes with UOMs for MSCFY12

namespace Arya
{
    using Framework.IO;
    using Arya.HelperForms;

    public partial class FrmImportData : Form
    {
        #region Fields (6)

        private readonly List<ListViewItem> _dataInputFields = new List<ListViewItem>
                                                                   {
                                                                       new ListViewItem(new[] {Yes, "Item Id", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 1", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 2", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 3", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 4", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 5", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 6", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 7", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 8", Blank}),
                                                                       new ListViewItem(new[] {No, "Taxonomy 9", Blank}),
                                                                       new ListViewItem(
                                                                           new[] {No, "Attribute Name", Blank}),
                                                                       new ListViewItem(
                                                                           new[] {No, "Attribute Type", Blank}),
                                                                       new ListViewItem(new[] {No, "Value", Blank}),
                                                                       new ListViewItem(new[] {No, "UOM", Blank}),
                                                                       new ListViewItem(new[] {No, "Field 1", Blank}),
                                                                       new ListViewItem(new[] {No, "Field 2", Blank}),
                                                                       new ListViewItem(new[] {No, "Field 3", Blank}),
                                                                       new ListViewItem(new[] {No, "Field 4", Blank}),
                                                                       new ListViewItem(new[] {No, "Field 5", Blank})
                                                                   };
        private string _delimiter;
        private ImportWorker _worker;
        private const string Blank = "";
        private const string No = "";
        private const string Yes = "Yes";

        #endregion Fields

        #region Constructors (1)

        public FrmImportData()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;

            AryaTools.Instance.Forms.TreeForm.showRemarksToolStripMenuItem.Checked = true;

            ShowHideFormFields();
        }

        #endregion Constructors

        #region Properties (1)

        public static decimal PauseBetweenSkus { get; set; }

        #endregion Properties

        #region Methods (30)

        // Private Methods (30) 

        private void btnAbort_Click(object sender, EventArgs e)
        {
            _worker.CurrentStatusCode = WorkerState.Abort;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            btnOpenInputFile.Enabled = false;
            btnLoad.Enabled = false;
            btnImport.Enabled = false;
            btnAbort.Enabled = true;

            ImportDataWorker.ImportType importType = rbInsertOnly.Checked
                                                         ? ImportDataWorker.ImportType.InsertSkusAndValues
                                                         : rbUpsertEntities.Checked
                                                               ? ImportDataWorker.ImportType.UpsertSkusAndValues
                                                               : rbDeleteExistingEntities.Checked
                                                                     ? ImportDataWorker.ImportType.
                                                                           UpdateDeleteExistingValues
                                                                     : ImportDataWorker.ImportType.
                                                                           UpsertSkusInsertValues;
            _worker = new ImportDataWorker(
                txtInputFilename.Text, _delimiter, _dataInputFields, txtClientName.Text, txtProjectName.Text,
                txtSetLoadName.Text, importType, chkMarkAsBefore.Checked, chkBoxNewSkus.Checked, chkAttributeNameUomAreOne.Checked);

            new Thread(_worker.UploadData) { IsBackground = true }.Start();
            statusTimer.Start();

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            TryLoadHeader();
        }

        private void btnOpenInputFile_Click(object sender, EventArgs e)
        {
            GetInputFileName();
        }

        private void BulkImport_FormClosing(object sender, FormClosingEventArgs e)
        {

            AryaTools.Instance.Forms.TreeForm.showRemarksToolStripMenuItem.Checked = false;
            while (_worker != null && _worker.CurrentStatusCode != WorkerState.Ready)
            {
                lblStatus.Text = @"Waiting for process to complete/abort";
                _worker.CurrentStatusCode = WorkerState.Abort;
                Refresh();
                Thread.Sleep(500);
            }
        }

        private void BulkImport_Load(object sender, EventArgs e)
        {
            cboDelimiter.SelectedIndex = 0;
            ResetMappings();
            EnableDisableLoadButton();
            EnableDisableImportButton();
            BringToFront();
        }

        private void cboDelimiter_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboDelimiter.Text)
            {
                case "Tab":
                    _delimiter = "\t";
                    break;

                case "Comma":
                    _delimiter = ",";
                    break;

                case "Pipe (|)":
                    _delimiter = "|";
                    break;

                default:
                    _delimiter = cboDelimiter.Text;
                    break;
            }
        }

        private void EnableDisableImportButton()
        {
            if (string.IsNullOrEmpty(txtProjectName.Text))
            {
                btnImport.Enabled = false;
                return;
            }

            if (
                lstMapping.Items.Cast<ListViewItem>().Any(
                    item => item.SubItems[0].Text.Equals(Yes) && item.SubItems[2].Text.Length == 0))
            {
                btnImport.Enabled = false;
                return;
            }

            btnImport.Enabled = true;
        }

        private void EnableDisableLoadButton()
        {
            btnLoad.Enabled = !string.IsNullOrEmpty(txtInputFilename.Text);
        }

        private void GetInputFileName()
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK || openFileDialog.FileName.Equals(txtInputFilename.Text))
                return;
            txtInputFilename.Text = openFileDialog.FileName;
            TryLoadHeader();
        }

        private ListViewItem GetMappingItemAtPoint(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.Text))
                return null;

            Point point = lstMapping.PointToClient(new Point(e.X, e.Y));
            return lstMapping.GetItemAt(point.X, point.Y);
        }

        private void lstFileFields_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstFileFields.SelectedIndex >= 0 &&
                lstFileFields.GetItemRectangle(lstFileFields.SelectedIndex).Contains(e.X, e.Y))
                DoDragDrop(lstFileFields.SelectedItem, DragDropEffects.Link);
        }

        private void lstMapping_DragDrop(object sender, DragEventArgs e)
        {
            ListViewItem item = GetMappingItemAtPoint(e);
            if (item == null)
                return;

            item.SubItems[2].Text = e.Data.GetData(DataFormats.Text).ToString();
            EnableDisableImportButton();
            lstMapping.Focus();
        }

        private void lstMapping_DragOver(object sender, DragEventArgs e)
        {
            ListViewItem item = GetMappingItemAtPoint(e);
            if (item == null)
                e.Effect = DragDropEffects.None;
            else
            {
                item.Selected = true;
                e.Effect = DragDropEffects.Link;
            }
        }

        private void lstMapping_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstMapping.SelectedItems.Count > 0)
                lstMapping.SelectedItems[0].SubItems[2].Text = Blank;
        }

        private void rbDeleteExistingEntities_CheckedChanged(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    @"Are you sure you want to delete all existing values?", @"Confirm Delete", MessageBoxButtons.YesNo) ==
                DialogResult.No)
                rbUpsertEntities.Checked = true;
        }

        private void rbInsertOnly_CheckedChanged(object sender, EventArgs e)
        {
            ShowHideUpdateActionGroupBox();
        }

        private void rbInsertUpdate_CheckedChanged(object sender, EventArgs e)
        {
            ShowHideUpdateActionGroupBox();
        }

        private IEnumerable<string> ReadHeaderFromFile(string fileName)
        {
            string[] header;
            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format("File [{0}] not found.", fileName));

            using (StreamReader reader = File.OpenText(fileName))
            {
                string firstLine = reader.ReadLine();
                header = firstLine.Split(new[] { _delimiter }, StringSplitOptions.None);
            }

            return header;
        }

        private void ResetMappings()
        {
            lstMapping.Items.Clear();
            _dataInputFields.ForEach(map => lstMapping.Items.Add(map));
        }

        private void ShowErrorLog()
        {
            string importId = string.Empty;
            try
            {
                importId = _worker.ImportId.ToString();
            }
            catch
            {
            }

            try
            {
                using (TextReader tr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Error.log"))
                {
                    string line, output = string.Empty;
                    while ((line = tr.ReadLine()) != null)
                        if (line.StartsWith(importId))
                            output += line + Environment.NewLine;

                    txtErrorLog.Text = output;
                    tabControlImportData.SelectTab(tpErrorLog);
                    tr.Close();
                }
            }
            catch
            {
            }
        }

        private void ShowHideFormFields()
        {
            txtClientName.Text = AryaTools.Instance.InstanceData.CurrentProject.ClientDescription;
            txtProjectName.Text = AryaTools.Instance.InstanceData.CurrentProject.ProjectName;
            txtSetLoadName.Text = AryaTools.Instance.InstanceData.CurrentProject.SetName;
        }

        private void ShowHideUpdateActionGroupBox()
        {
            grpUpdateAction.Enabled = rbInsertUpdate.Checked;
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            lblStatus.Text = _worker.CurrentStatus;

            if (_worker.CurrentStatusCode == WorkerState.Saving && btnAbort.Enabled)
                btnAbort.Enabled = false;
            else if (_worker.CurrentStatusCode == WorkerState.Ready)
            {
                statusTimer.Stop();
                btnOpenInputFile.Enabled = true;
                btnLoad.Enabled = true;
                btnImport.Enabled = true;
                btnAbort.Enabled = false;

                ShowErrorLog();
                EnableDisableImportButton();
                GC.Collect();
            }
        }

        private void TryLoadHeader()
        {
            try
            {
                lstFileFields.Items.Clear();
                ResetMappings();
                ReadHeaderFromFile(txtInputFilename.Text).ForEach(hdr => lstFileFields.Items.Add(hdr));
            }
            catch (FileNotFoundException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void txtClientName_TextChanged(object sender, EventArgs e)
        {
            EnableDisableImportButton();
        }

        private void txtInputFilename_Enter(object sender, EventArgs e)
        {
            if (txtInputFilename.Text.Length == 0)
                GetInputFileName();
        }

        private void txtInputFilename_TextChanged(object sender, EventArgs e)
        {
            EnableDisableLoadButton();
        }

        private void txtPauseDuration_ValueChanged(object sender, EventArgs e)
        {
            PauseBetweenSkus = txtPauseDuration.Value;
        }

        private void txtProjectName_TextChanged(object sender, EventArgs e)
        {
            EnableDisableImportButton();
        }

        #endregion Methods
    }

    internal abstract class ImportWorker
    {
        #region Fields (6)

        public string CurrentStatus = string.Empty;
        public WorkerState CurrentStatusCode = WorkerState.Working;
        protected string FieldDelimiter;
        protected IEnumerable<ListViewItem> FieldMappings;
        public Guid ImportId = Guid.NewGuid();
        protected string InputFilePath;

        #endregion Fields

        #region Constructors (1)

        internal ImportWorker()
        {
            AryaTools.Instance.SubmitAllChanges();
            //AryaTools.Instance.InitAryaTools();
        }

        #endregion Constructors

        #region Methods (3)

        // Public Methods (1) 

        public abstract void UploadData();
        // Protected Methods (2) 

        protected static string ExceptionToString(Exception ex)
        {
            string msg = ex.Message;

            Exception innerException = ex.InnerException;
            if (innerException != null)
                msg += Environment.NewLine + ExceptionToString(innerException);

            return msg;
        }

        protected void GetMappingPositions(string[] header)
        {
            foreach (ListViewItem fieldMapping in FieldMappings)
            {
                if (fieldMapping.SubItems[2].Text.Length <= 0)
                    continue;

                FieldInfo field = GetType().GetField(
                    "_pos" + fieldMapping.SubItems[1].Text.Replace(" ", string.Empty),
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                int mapIndex = Array.IndexOf(header, fieldMapping.SubItems[2].Text);
                field.SetValue(this, mapIndex);
            }
        }

        #endregion Methods
    }

    internal class ImportDataWorker : ImportWorker
    {
        #region Fields (22)

        private const string Blank = "";
        private readonly string _clientName;
        private readonly ImportType _importType;
        private readonly string _projectName;
        private readonly string _setName;
        private int _posAttributeType = -1;
        private int _posAttributeName = -1;
        private int _posField1 = -1;
        private int _posField2 = -1;
        private int _posField3 = -1;
        private int _posField4 = -1;
        private int _posField5 = -1;
        private int _posItemId = -1;
        private int _posTaxonomy1 = -1;
        private int _posTaxonomy2 = -1;
        private int _posTaxonomy3 = -1;
        private int _posTaxonomy4 = -1;
        private int _posTaxonomy5 = -1;
        private int _posTaxonomy6 = -1;
        private int _posTaxonomy7 = -1;
        private int _posTaxonomy8 = -1;
        private int _posTaxonomy9 = -1;
        private int _posUom = -1;
        private int _posValue = -1;
        private string currentDataBase;
        private bool attributeNameUomAreOne = false;
        private bool _isNewSkuAllowed;
        private string databaseName = AryaTools.Instance.InstanceData.CurrentProject.DatabaseName;
        #endregion Fields

        #region ImportType enum

        public enum ImportType
        {
            InsertSkusAndValues,
            UpsertSkusInsertValues,
            UpsertSkusAndValues,
            UpdateDeleteExistingValues
        }

        #endregion

        #region Constructors (1)

        internal ImportDataWorker(
            string filePath, string delimiter, IEnumerable<ListViewItem> mappings, string clientDescription,
            string projectName, string setName, ImportType importType, bool markAsBefore, bool newskus, bool attributeNameUomAreOne = false)
        {
            InputFilePath = filePath;
            FieldDelimiter = delimiter;
            FieldMappings = mappings;
            _clientName = clientDescription;
            _projectName = projectName;
            _setName = setName;
            _importType = importType;
            _markAsBefore = markAsBefore;
            _isNewSkuAllowed = newskus;
            this.attributeNameUomAreOne = attributeNameUomAreOne;
            currentDataBase = databaseName;
        }

        #endregion Constructors

        #region Methods (8)

        // Public Methods (1) 

        private readonly Dictionary<string, Attribute> _allAttributes = new Dictionary<string, Attribute>();
        // AttributeNameString, Attribute

        private readonly Dictionary<string, Sku> _allSkus = new Dictionary<string, Sku>(); // ItemIDString, Sku

        private readonly Dictionary<string, TaxonomyInfo> _allTaxonomies = new Dictionary<string, TaxonomyInfo>();
        private readonly bool _markAsBefore;
        //FullPath, Taxonomy

        private readonly Dictionary<string, TaxonomyInfo> _skuTaxonomies = new Dictionary<string, TaxonomyInfo>();
        //ItemIDString, Taxonomy

        private Project _project;

        private int GetMaxPos()
        {
            int maxPos = -1;
            if (_posAttributeName > maxPos)
                maxPos = _posAttributeName;
            if (_posAttributeType > maxPos)
                maxPos = _posAttributeType;
            if (_posField1 > maxPos)
                maxPos = _posField1;
            if (_posField2 > maxPos)
                maxPos = _posField2;
            if (_posField3 > maxPos)
                maxPos = _posField3;
            if (_posField4 > maxPos)
                maxPos = _posField4;
            if (_posField5 > maxPos)
                maxPos = _posField5;
            if (_posItemId > maxPos)
                maxPos = _posItemId;
            if (_posTaxonomy1 > maxPos)
                maxPos = _posTaxonomy1;
            if (_posTaxonomy2 > maxPos)
                maxPos = _posTaxonomy2;
            if (_posTaxonomy3 > maxPos)
                maxPos = _posTaxonomy3;
            if (_posTaxonomy4 > maxPos)
                maxPos = _posTaxonomy4;
            if (_posTaxonomy5 > maxPos)
                maxPos = _posTaxonomy5;
            if (_posTaxonomy6 > maxPos)
                maxPos = _posTaxonomy6;
            if (_posTaxonomy7 > maxPos)
                maxPos = _posTaxonomy7;
            if (_posTaxonomy8 > maxPos)
                maxPos = _posTaxonomy8;
            if (_posTaxonomy9 > maxPos)
                maxPos = _posTaxonomy9;
            if (_posUom > maxPos)
                maxPos = _posUom;
            if (_posValue > maxPos)
                maxPos = _posValue;

            return maxPos;
        }

        //private static Encoding GetFileEncoding(string filePath)
        //{
        //    Encoding enc;
        //    var file = new FileStream(filePath,
        //        FileMode.Open, FileAccess.Read, FileShare.Read);
        //    if (file.CanSeek)
        //    {
        //        var bom = new byte[4]; // Get the byte-order mark, if there is one 
        //        file.Read(bom, 0, 4);
        //        if ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) || // utf-8 
        //            (bom[0] == 0xff && bom[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le 
        //            (bom[0] == 0xfe && bom[1] == 0xff) || // utf-16 and ucs-2 
        //            (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)) // ucs-4 
        //        {
        //            enc = Encoding.Unicode;
        //        }
        //        else
        //        {
        //            enc = Encoding.ASCII;
        //        }

        //        // Now reposition the file cursor back to the start of the file 
        //        file.Seek(0, SeekOrigin.Begin);
        //    }
        //    else
        //    {
        //        // The file cannot be randomly accessed, so you need to decide what to set the default to 
        //        // based on the data provided. If you're expecting data from a lot of older applications, 
        //        // default your encoding to Encoding.ASCII. If you're expecting data from a lot of newer 
        //        // applications, default your encoding to Encoding.Unicode. Also, since binary files are 
        //        // single byte-based, so you will want to use Encoding.ASCII, even though you'll probably 
        //        // never need to use the encoding then since the Encoding classes are really meant to get 
        //        // strings from the byte array that is the file. 
        //        enc = Encoding.ASCII;
        //    }

        //    file.Close();
        //    file.Dispose();

        //    return enc;
        //}
        public override void UploadData()
        {
            var fileEncoding = FileHelper.GetFileEncoding(InputFilePath);//GetFileEncoding(InputFilePath);
            if (fileEncoding != Encoding.Unicode)
            {
                if (
                    MessageBox.Show(
                        string.Format("File Encoding is {0}(not UTF-8 or Unicode). Import anyway?", fileEncoding), "Unexpected File Encoding",
                        MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    CurrentStatus = "Aborted.";
                    CurrentStatusCode = WorkerState.Ready;
                    return;
                }
            }

            int entityInsertCount = 0, entityUpdateCount = 0, entityUnchangedCount = 0, skuInsertCount = 0,
                skuUpdateCount = 0, taxonomyCount = 0;

            var deactivatedItemAttributes = new Dictionary<Guid, HashSet<string>>();

            //try
            //{


            TextWriter tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Error.log", true);
            tw.WriteLine(
                string.Format("{1} Import started at {0}. Input: {2}", DateTime.Now, ImportId, InputFilePath));
            tw.Close();

            using (StreamReader reader = File.OpenText(InputFilePath))
            {
                CurrentStatus = "Reading Header";
                string line = reader.ReadLine();
                string[] header = line.Split(new[] { FieldDelimiter }, StringSplitOptions.None);

                // Get Positions for the various fields in the text file
                GetMappingPositions(header);


                if (attributeNameUomAreOne)
                {
                    //even if user wants to combine, attributename and Uom, but did not provide uom column.
                    if (_posUom == -1)
                    {
                        attributeNameUomAreOne = false;
                    }
                }

                //First, create the _project, if necessary
                GetProject(header);

                //AryaTools.Instance.InitAryaTools();

                int maxPosition = GetMaxPos();
                int lineCount = 0, lastLineCount = 0;
                string previousItemId = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new[] { FieldDelimiter }, StringSplitOptions.None);
                    if (parts.Length < maxPosition)
                    {
                        try
                        {
                            tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Error.log", true);
                            tw.WriteLine(
                                string.Format(
                                    "{0} {1}: Ignoring Line due to insufficient values:{2}{3}", ImportId,
                                    DateTime.Now, Environment.NewLine, line));
                            tw.Close();
                        }
                        catch (Exception)
                        {
                        }

                        ++lineCount;
                        continue;
                    }
                    string itemId = parts[_posItemId].Trim();

                    if (!previousItemId.Equals(itemId))
                    {
                        CurrentStatus = GetCurrentStatus(
                            entityInsertCount, entityUpdateCount, entityUnchangedCount, skuInsertCount,
                            skuUpdateCount, taxonomyCount);
                        //AryaTools.Instance.InstanceData.Dc.SubmitChanges();
                        AryaTools.Instance.InstanceData.Dc.SubmitChanges();

                        if (lineCount > lastLineCount + 500)
                        {
                            lastLineCount = lineCount;
                            try
                            {
                                tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Error.log", true);
                                tw.WriteLine(
                                    string.Format(
                                        "{0} {1}: {2}", ImportId, DateTime.Now,
                                        CurrentStatus.Replace(Environment.NewLine, "; ")));
                                tw.Close();
                            }
                            catch (Exception)
                            {
                            }
                        }

                        if (FrmImportData.PauseBetweenSkus > 0)
                            Thread.Sleep((int)(FrmImportData.PauseBetweenSkus * 1000));
                    }

                    if (string.IsNullOrEmpty(itemId))
                        break;
                    previousItemId = itemId;

                    bool newTaxonomy;
                    TaxonomyInfo taxonomy = GetTaxonomy(
                        _posTaxonomy1 >= 0 ? parts[_posTaxonomy1].Trim() : Blank,
                        _posTaxonomy2 >= 0 ? parts[_posTaxonomy2].Trim() : Blank,
                        _posTaxonomy3 >= 0 ? parts[_posTaxonomy3].Trim() : Blank,
                        _posTaxonomy4 >= 0 ? parts[_posTaxonomy4].Trim() : Blank,
                        _posTaxonomy5 >= 0 ? parts[_posTaxonomy5].Trim() : Blank,
                        _posTaxonomy6 >= 0 ? parts[_posTaxonomy6].Trim() : Blank,
                        _posTaxonomy7 >= 0 ? parts[_posTaxonomy7].Trim() : Blank,
                        _posTaxonomy8 >= 0 ? parts[_posTaxonomy8].Trim() : Blank,
                        _posTaxonomy9 >= 0 ? parts[_posTaxonomy9].Trim() : Blank,
                        out newTaxonomy);
                    if (newTaxonomy)
                        taxonomyCount++;

                    //See if the Sku exists
                    Sku currentSku = null;
                    if (_allSkus.ContainsKey(itemId))
                        currentSku = _allSkus[itemId];
                    else if (_importType != ImportType.InsertSkusAndValues)
                    {
                        //currentSku =
                        //    AryaTools.Instance.InstanceData.Dc.Skus.FirstOrDefault(sku => sku.ProjectID.Equals(_project.ID) && sku.ItemID.Equals(itemId));
                        currentSku =
                           AryaTools.Instance.InstanceData.Dc.Skus.FirstOrDefault(sku => sku.ProjectID.Equals(_project.ID) && sku.ItemID.Equals(itemId));
                        if (currentSku != null)
                        {
                            skuUpdateCount++;
                            _allSkus.Add(itemId, currentSku);
                            if (_importType == ImportType.UpdateDeleteExistingValues)
                            {
                                EntitySet<EntityInfo> entityInfos = currentSku.EntityInfos;
                                //AryaTools.Instance.InstanceData.Dc.EntityInfos.DeleteAllOnSubmit(entityInfos);
                                //AryaTools.Instance.InstanceData.Dc.SubmitChanges();
                                AryaTools.Instance.InstanceData.Dc.EntityInfos.DeleteAllOnSubmit(entityInfos);
                                AryaTools.Instance.InstanceData.Dc.SubmitChanges();
                            }
                        }
                    }

                    if (currentSku != null)
                    {
                        if (_posTaxonomy1 >= 0)
                        {
                            if (!_skuTaxonomies.ContainsKey(itemId))
                            {
                                TaxonomyInfo tax = currentSku.Taxonomy;
                                if (tax != null)
                                    _skuTaxonomies.Add(itemId, tax);
                            }
                            if (!_skuTaxonomies.ContainsKey(itemId) || _skuTaxonomies[itemId].ID != taxonomy.ID)
                            {
                                currentSku.SkuInfos.ForEach(si => si.Active = false);
                                currentSku.SkuInfos.Add(new SkuInfo { TaxonomyID = taxonomy.ID });
                            }
                        }
                    }
                    else
                    {
                        if (!_isNewSkuAllowed)
                        {

                            try
                            {
                                tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Error.log", true);
                                tw.WriteLine(
                                    string.Format(
                                        "{0} {1}:Skipped (as the sku does not already exist):{2}{3}", ImportId,
                                        DateTime.Now, Environment.NewLine, line));
                                tw.Close();
                            }
                            catch (Exception)
                            {
                            }
                            continue;

                        }

                        // Create a New SKU
                        currentSku = new Sku
                        {
                            ItemID = itemId,
                            SkuInfos = { new SkuInfo { TaxonomyID = taxonomy.ID } },
                            ProjectID = _project.ID,
                            SkuType = "Product"
                        };
                        //AryaTools.Instance.InstanceData.Dc.Skus.InsertOnSubmit(currentSku);
                        AryaTools.Instance.InstanceData.Dc.Skus.InsertOnSubmit(currentSku);
                        currentSku.ID = currentSku.ID;
                        _allSkus.Add(itemId, currentSku);
                        _skuTaxonomies.Add(itemId, taxonomy);
                        skuInsertCount++;
                    }

                    if (_posAttributeName >= 0 && _posValue >= 0)
                    {
                        //--Check here for Uom as well
                        string attributeName = parts[_posAttributeName].Trim();
                        string attributeType = _posAttributeType >= 0
                                                   ? parts[_posAttributeType].Trim()
                                                   : string.Empty;

                        if (!deactivatedItemAttributes.ContainsKey(currentSku.ID))
                            deactivatedItemAttributes.Add(currentSku.ID, new HashSet<string>());

                        HashSet<string> deactivatedAttributes = deactivatedItemAttributes[currentSku.ID];
                        // Create the Entity Data and add it to the SKU
                        Guid attributeId =
                            GetAttribute(attributeName, attributeType, this._posUom >= 0);
                        EntityData entityData = CreateEntityData(parts, attributeId);

                        var attributeNameToCheck = attributeNameUomAreOne && !string.IsNullOrWhiteSpace(entityData.Uom)
                                                       ? attributeName + entityData.Uom
                                                       : attributeName;

                        bool doUpdate = false;
                        List<EntityInfo> existingEntityInfos = null;
                        if (_importType == ImportType.UpsertSkusAndValues &&
                            !deactivatedAttributes.Contains(attributeNameToCheck))
                        {
                            //check if this attribute already exists. if it does, then update; else, insert a new entityinfo
                            //existingEntityInfos = (from ed in AryaTools.Instance.InstanceData.Dc.EntityDatas
                            //                       where
                            //                           ed.Active && ed.AttributeID.Equals(attributeId) &&
                            //                           ed.EntityInfo.SkuID.Equals(currentSku.ID)
                            //                       select ed.EntityInfo).ToList();

                            if (attributeNameUomAreOne && !string.IsNullOrWhiteSpace(entityData.Uom))
                            {
                                existingEntityInfos = (from ed in AryaTools.Instance.InstanceData.Dc.EntityDatas
                                                       where
                                                           ed.Active && ed.AttributeID.Equals(attributeId) &&
                                                           ed.Uom == entityData.Uom &&
                                                           ed.EntityInfo.SkuID.Equals(currentSku.ID)
                                                       select ed.EntityInfo).ToList();
                            }
                            else
                            {

                                existingEntityInfos = (from ed in AryaTools.Instance.InstanceData.Dc.EntityDatas
                                                       where
                                                           ed.Active && ed.AttributeID.Equals(attributeId) &&
                                                           ed.EntityInfo.SkuID.Equals(currentSku.ID)
                                                       select ed.EntityInfo).ToList();
                            }

                            if (existingEntityInfos.Count > 1)
                                existingEntityInfos.SelectMany(ei => ei.EntityDatas).ForEach(
                                    ed => ed.Active = false);

                            doUpdate = existingEntityInfos.Count == 1;
                            deactivatedAttributes.Add(attributeNameToCheck);
                        }

                        if (doUpdate)
                        {
                            //Fix the UOM Parsed field.
                            IEnumerable<EntityData> entityDatas =
                                existingEntityInfos.SelectMany(
                                    ei => ei.EntityDatas.Where(ed => ed.Active && ed.Value.Equals(entityData.Value)));
                            if (_posUom >= 0 && entityData.Uom != null)
                                entityDatas = entityDatas.Where(ed => ed.Uom != null && ed.Uom.Equals(entityData.Uom));
                            if (_posField1 >= 0)
                                entityDatas = entityDatas.Where(ed => ed.Field1 != null && ed.Field1.Equals(entityData.Field1));
                            if (_posField2 >= 0)
                                entityDatas = entityDatas.Where(ed => ed.Field2 != null && ed.Field2.Equals(entityData.Field2));
                            if (_posField3 >= 0)
                                entityDatas = entityDatas.Where(ed => ed.Field3 != null && ed.Field3.Equals(entityData.Field3));
                            //if (_posField4 >= 0)
                            //    entityDatas = entityDatas.Where(ed => ed.Field4 != null && ed.Field4.Equals(entityData.Field4));
                            if (_posField5 >= 0)
                                entityDatas = entityDatas.Where(ed => ed.Field5 != null && ed.Field5.Equals(entityData.Field5));

                            entityDatas = entityDatas.ToList();

                            bool valueMatches = entityDatas.Any();

                            if (valueMatches)
                            {
                                entityUnchangedCount++;

                                if (_markAsBefore)
                                {
                                    existingEntityInfos.SelectMany(ei => ei.EntityDatas).ForEach(
                                        ed => ed.BeforeEntity = false);
                                    entityDatas.ForEach(ed => ed.BeforeEntity = _markAsBefore);
                                }
                            }
                            else
                            {
                                // First disable all existing values
                                // Mark exisitng values as Not BeforeEntity if new EntityData is marked as BeforeEntity
                                existingEntityInfos.SelectMany(ei => ei.EntityDatas).ForEach(
                                    ed =>
                                    {
                                        ed.Active = false;
                                        if (_markAsBefore)
                                            ed.BeforeEntity = false;
                                    });

                                EntityInfo entityInfo = existingEntityInfos.First();
                                entityInfo.EntityDatas.Add(entityData);
                                entityUpdateCount++;
                            }
                        }
                        else
                        {
                            //AryaTools.Instance.InstanceData.Dc.EntityInfos.InsertOnSubmit(
                            //    new EntityInfo { SkuID = currentSku.ID, EntityDatas = { entityData } });
                            AryaTools.Instance.InstanceData.Dc.EntityInfos.InsertOnSubmit(
                                new EntityInfo { SkuID = currentSku.ID, EntityDatas = { entityData } });
                            entityInsertCount++;
                        }
                    }

                    ++lineCount;
                    CurrentStatus = GetCurrentStatus(
                        entityInsertCount, entityUpdateCount, entityUnchangedCount, skuInsertCount, skuUpdateCount,
                        taxonomyCount);

                    //Check if I have to abort this process
                    if (CurrentStatusCode != WorkerState.Abort)
                        continue;

                    CurrentStatus = "Aborted.";
                    CurrentStatusCode = WorkerState.Ready;
                    return;
                }

                // Submit all changes
                CurrentStatus = GetCurrentStatus(
                    entityInsertCount, entityUpdateCount, entityUnchangedCount, skuInsertCount, skuUpdateCount,
                    taxonomyCount);
                try
                {
                    tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Error.log", true);
                    tw.WriteLine(
                        string.Format(
                            "{0} {1}: {2}", ImportId, DateTime.Now, CurrentStatus.Replace(Environment.NewLine, "; ")));
                    tw.Close();
                }
                catch (Exception)
                {
                }
                CurrentStatusCode = WorkerState.Saving;
                //AryaTools.Instance.InstanceData.Dc.SubmitChanges();
                AryaTools.Instance.InstanceData.Dc.SubmitChanges();
                //AryaTools.Instance.InitAryaTools();
            } //Close the file

            CurrentStatus = "Done!";

            CurrentStatusCode = WorkerState.Ready;
            GC.Collect();
        }

        private static string GetCurrentStatus(
            int entityInsertCount, int entityUpdateCount, int entityUnchangedCount, int skuInsertCount,
            int skuUpdateCount, int taxonomyCount)
        {
            string inserts = string.Empty;
            string updates = string.Empty;
            string status = string.Empty;

            if (taxonomyCount > 0)
                inserts += string.Format(
                    "{0}{1} Taxonomy Nodes", inserts.Length > 0 ? ", " : string.Empty, taxonomyCount);
            if (skuInsertCount > 0)
                inserts += string.Format("{0}{1} SKUs", inserts.Length > 0 ? ", " : string.Empty, skuInsertCount);
            if (entityInsertCount > 0)
                inserts += string.Format(
                    "{0}{1} Values Inserted", inserts.Length > 0 ? ", " : string.Empty, entityInsertCount);

            if (skuUpdateCount > 0)
                updates += string.Format("{0}{1} SKUs", updates.Length > 0 ? ", " : string.Empty, skuUpdateCount);
            if (entityUpdateCount > 0)
                updates += string.Format(
                    "{0}{1} Values Updated", updates.Length > 0 ? ", " : string.Empty, entityUpdateCount);
            if (entityUnchangedCount > 0)
                updates += string.Format(
                    "{0}{1} Values Unchanged", updates.Length > 0 ? ", " : string.Empty, entityUnchangedCount);

            if (inserts.Length > 0)
                status += inserts;
            if (updates.Length > 0)
                status += (status.Length > 0 ? Environment.NewLine : string.Empty) + updates;
            if (string.IsNullOrEmpty(status))
                status = "Working...";

            return status;
        }

        // Private Methods (7) 

        private EntityData CreateEntityData(string[] parts, Guid attributeId)
        {
            return new EntityData
            {
                AttributeID = attributeId,
                Value = parts[_posValue].Trim(),
                Uom = _posUom > -1 && parts[_posUom].Trim().Length > 0 ? parts[_posUom].Trim() : null,
                Field1 = _posField1 > -1 ? parts[_posField1].Trim() : null,
                Field2 = _posField2 > -1 ? parts[_posField2].Trim() : null,
                Field3 = _posField3 > -1 ? parts[_posField3].Trim() : null,
                //Field4 = _posField4 >= 0 ? parts[_posField4].Trim() : null,
                //Field4 = BaseUnitConversion.GetBaseUnitValueAsString(parts[_posValue].Trim(), _posUom >= 0 ? parts[_posUom].Trim() : null),
                Field5 = _posField5 > -1 ? parts[_posField5].Trim() : null,
                BeforeEntity = _markAsBefore
            };

        }

        private Guid GetAttribute(string attributeName, string attributeType, bool hasUom)
        {
            //If an attribute is found, return it
            string attributeKey = attributeName.ToLower();
            if (_allAttributes.ContainsKey(attributeKey))
            {
                Attribute att = _allAttributes[attributeKey];
                if (!string.IsNullOrEmpty(attributeType))
                    att.AttributeType = attributeType;
                return att.ID;
            }

            //Build a new taxonomy node and add to database
            var newAttribute = new Attribute
            {
                AttributeName = attributeName,
                ProjectID = _project.ID
            };

            if (!string.IsNullOrEmpty(attributeType))
                newAttribute.AttributeType = attributeType;
            //AryaTools.Instance.InstanceData.Dc.Attributes.InsertOnSubmit(newAttribute);
            AryaTools.Instance.InstanceData.Dc.Attributes.InsertOnSubmit(newAttribute);
            _allAttributes.Add(attributeKey, newAttribute);

            return newAttribute.ID;
        }

        private void GetProject(IList<string> header)
        {
            CurrentStatus = "Fetching Project Data";
            var projects = (from prj in AryaTools.Instance.InstanceData.Dc.Projects
                            where prj.ProjectName.Equals(_projectName)
                            select prj).ToList();

            if (projects.Any())
            {
                _project = projects.First();

                //Update any Project Properties (if necessary)
                //Causing too many problems, we will phase this out.
                //if (!string.IsNullOrEmpty(_clientName))
                //    _project.ClientDescription = _clientName;
                //if (!string.IsNullOrEmpty(_setName))
                //    _project.SetName = _setName;
                //if (_posField1 >= 0 && !string.IsNullOrEmpty(_project.EntityField1Name))
                //    _project.EntityField1Name = header[_posField1];
                //if (_posField2 >= 0 && !string.IsNullOrEmpty(_project.EntityField2Name))
                //    _project.EntityField2Name = header[_posField2];
                //if (_posField3 >= 0 && !string.IsNullOrEmpty(_project.EntityField3Name))
                //    _project.EntityField3Name = header[_posField3];
                //if (_posField4 >= 0 && !string.IsNullOrEmpty(_project.EntityField4Name))
                //    _project.EntityField4Name = header[_posField4];
                //if (_posField5 >= 0 && !string.IsNullOrEmpty(_project.EntityField5Name))
                //    _project.EntityField5Name = header[_posField5];

                //Fetch all existing attributes
                CurrentStatus = "Fetching Project Data - Existing Attributes";
                _project.Attributes.Where(attr => attr.AttributeType == AttributeTypeEnum.Sku.ToString() || attr.AttributeType == AttributeTypeEnum.Global.ToString() || attr.AttributeType == AttributeTypeEnum.Derived.ToString()).ForEach(
                    att =>
                    {
                        string attributeKey = att.AttributeName.ToLower();
                        if (!_allAttributes.ContainsKey(attributeKey))
                            _allAttributes.Add(attributeKey, att);
                    });

                //Fetch all existing taxonomies
                CurrentStatus = "Fetching Project Data - Existing Taxonomy Nodes";
                _project.TaxonomyInfos.Where(ti => ti.TaxonomyData != null && ti.NodeType == TaxonomyInfo.NodeTypeRegular).ForEach(
                    tax =>
                    {
                        string taxString = tax.ToString();
                        if (!_allTaxonomies.ContainsKey(taxString))
                            _allTaxonomies.Add(taxString, tax);
                    });
            }
            else
            {
                _project = new Project
                {
                    ProjectName = _projectName,
                    ClientDescription = _clientName,
                    SetName = _setName,
                    EntityField1Name = _posField1 >= 0 ? header[_posField1] : null,
                    EntityField2Name = _posField2 >= 0 ? header[_posField2] : null,
                    EntityField3Name = _posField3 >= 0 ? header[_posField3] : null,
                    EntityField4Name = _posField4 >= 0 ? header[_posField4] : null,
                    EntityField5Name = _posField5 >= 0 ? header[_posField5] : null,
                    UserProjects = { new UserProject { UserID = AryaTools.Instance.InstanceData.CurrentUser.ID } }
                };
                AryaTools.Instance.InstanceData.Dc.Projects.InsertOnSubmit(_project);
            }

            //AryaTools.Instance.InstanceData.Dc.SubmitChanges();
            AryaTools.Instance.InstanceData.Dc.SubmitChanges();
        }

        private TaxonomyInfo GetTaxonomy(
            string tax1, string tax2, string tax3, string tax4, string tax5, string tax6, string tax7, string tax8, string tax9, out bool newlyCreated)
        {
            string taxonomy1, taxonomy2 = string.Empty, taxonomy3 = string.Empty, taxonomy4 = string.Empty,
                   taxonomy5 = string.Empty, taxonomy6 = string.Empty, taxonomy7 = string.Empty,
                   taxonomy8 = string.Empty, taxonomy9 = string.Empty;
            int level = 1;
            string taxonomy = taxonomy1 = tax1;
            if (!string.IsNullOrEmpty(tax2))
            {
                level = 2;
                taxonomy = taxonomy2 = taxonomy1 + TaxonomyInfo.Delimiter + tax2;
                if (!string.IsNullOrEmpty(tax3))
                {
                    level = 3;
                    taxonomy = taxonomy3 = taxonomy2 + TaxonomyInfo.Delimiter + tax3;
                    if (!string.IsNullOrEmpty(tax4))
                    {
                        level = 4;
                        taxonomy = taxonomy4 = taxonomy3 + TaxonomyInfo.Delimiter + tax4;
                        if (!string.IsNullOrEmpty(tax5))
                        {
                            level = 5;
                            taxonomy = taxonomy5 = taxonomy4 + TaxonomyInfo.Delimiter + tax5;
                            if (!string.IsNullOrEmpty(tax6))
                            {
                                level = 6;
                                taxonomy = taxonomy6 = taxonomy5 + TaxonomyInfo.Delimiter + tax6;
                                if (!string.IsNullOrEmpty(tax7))
                                {
                                    level = 7;
                                    taxonomy = taxonomy7 = taxonomy6 + TaxonomyInfo.Delimiter + tax7;
                                    if (!string.IsNullOrEmpty(tax8))
                                    {
                                        level = 8;
                                        taxonomy = taxonomy8 = taxonomy7 + TaxonomyInfo.Delimiter + tax8;
                                        if (!string.IsNullOrEmpty(tax9))
                                        {
                                            level = 9;
                                            taxonomy = taxonomy9 = taxonomy8 + TaxonomyInfo.Delimiter + tax9;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //If a taxonomy node is found, return it
            if (_allTaxonomies.ContainsKey(taxonomy))
            {
                newlyCreated = false;
                return _allTaxonomies[taxonomy];
            }

            newlyCreated = true;

            //Build and return the taxonomy node
            TaxonomyInfo taxLevel1;
            if (_allTaxonomies.ContainsKey(taxonomy1))
                taxLevel1 = _allTaxonomies[taxonomy1];
            else
            {
                taxLevel1 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular"};
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel1);
                taxLevel1.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax1 });
                _allTaxonomies.Add(taxonomy1, taxLevel1);
            }
            if (level == 1)
                return taxLevel1;

            TaxonomyInfo taxLevel2;
            if (_allTaxonomies.ContainsKey(taxonomy2))
                taxLevel2 = _allTaxonomies[taxonomy2];
            else
            {
                taxLevel2 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular" };
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel2);
                taxLevel2.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax2, ParentTaxonomyID = taxLevel1.ID });
                _allTaxonomies.Add(taxonomy2, taxLevel2);
            }
            if (level == 2)
                return taxLevel2;

            TaxonomyInfo taxLevel3;
            if (_allTaxonomies.ContainsKey(taxonomy3))
                taxLevel3 = _allTaxonomies[taxonomy3];
            else
            {
                taxLevel3 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular" };
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel3);
                taxLevel3.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax3, ParentTaxonomyID = taxLevel2.ID });
                _allTaxonomies.Add(taxonomy3, taxLevel3);
            }
            if (level == 3)
                return taxLevel3;

            TaxonomyInfo taxLevel4;
            if (_allTaxonomies.ContainsKey(taxonomy4))
                taxLevel4 = _allTaxonomies[taxonomy4];
            else
            {
                taxLevel4 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular" };
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel4);
                taxLevel4.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax4, ParentTaxonomyID = taxLevel3.ID });
                _allTaxonomies.Add(taxonomy4, taxLevel4);
            }
            if (level == 4)
                return taxLevel4;

            TaxonomyInfo taxLevel5;
            if (_allTaxonomies.ContainsKey(taxonomy5))
                taxLevel5 = _allTaxonomies[taxonomy5];
            else
            {
                taxLevel5 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular" };
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel5);
                taxLevel5.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax5, ParentTaxonomyID = taxLevel4.ID });
                _allTaxonomies.Add(taxonomy5, taxLevel5);
            }
            if (level == 5)
                return taxLevel5;

            TaxonomyInfo taxLevel6;
            if (_allTaxonomies.ContainsKey(taxonomy6))
                taxLevel6 = _allTaxonomies[taxonomy6];
            else
            {
                taxLevel6 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular" };
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel6);
                taxLevel6.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax6, ParentTaxonomyID = taxLevel5.ID });
                _allTaxonomies.Add(taxonomy6, taxLevel6);
            }
            if (level == 6)
                return taxLevel6;

            TaxonomyInfo taxLevel7;
            if (_allTaxonomies.ContainsKey(taxonomy7))
                taxLevel7 = _allTaxonomies[taxonomy7];
            else
            {
                taxLevel7 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular" };
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel7);
                taxLevel7.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax7, ParentTaxonomyID = taxLevel6.ID });
                _allTaxonomies.Add(taxonomy7, taxLevel7);
            }
            if (level == 7)
                return taxLevel7;

            TaxonomyInfo taxLevel8;
            if (_allTaxonomies.ContainsKey(taxonomy8))
                taxLevel8 = _allTaxonomies[taxonomy8];
            else
            {
                taxLevel8 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular" };
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel8);
                taxLevel8.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax8, ParentTaxonomyID = taxLevel7.ID });
                _allTaxonomies.Add(taxonomy8, taxLevel8);
            }
            if (level == 8)
                return taxLevel8;

            TaxonomyInfo taxLevel9;
            if (_allTaxonomies.ContainsKey(taxonomy9))
                taxLevel9 = _allTaxonomies[taxonomy9];
            else
            {
                taxLevel9 = new TaxonomyInfo { ShowInTree = true, ProjectID = _project.ID, NodeType = "Regular" };
                AryaTools.Instance.InstanceData.Dc.TaxonomyInfos.InsertOnSubmit(taxLevel9);
                taxLevel9.TaxonomyDatas.Add(new TaxonomyData { NodeName = tax9, ParentTaxonomyID = taxLevel8.ID });
                _allTaxonomies.Add(taxonomy9, taxLevel9);
            }
            if (level == 9)
                return taxLevel9;

            return null;//What the hell? How did I get to this line!!!???
        }

        #endregion Methods
    }
}