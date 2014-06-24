using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Arya.Data;
using Arya.Framework.Collections;
using Arya.Framework.Extensions;
using Arya.HelperClasses;
using Arya.Properties;
namespace Arya
{
    using Framework4.Collections;
    using Framework4.ComponentModel;
    using Framework4.IO.Exports;

    public partial class FrmExportDataNew : Form
    {
        private ExportWorker worker;
        private readonly string[] skuCollection;
        private readonly TaxonomyCollection taxonomyCollection;

        public FrmExportDataNew()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
        }

        public FrmExportDataNew(IEnumerable<string> skus)
            : this()
        {
            skuCollection = skus.ToArray();
        }

        public FrmExportDataNew(List<TaxonomyInfo> taxonomies) : this()
        {
            taxonomyCollection = new TaxonomyCollection();
            taxonomies.ForEach(p => taxonomyCollection.Add(new ExtendedTaxonomyInfo(p)));
        }

        private void FrmExportDataNew_Load(object sender, EventArgs e)
        {
            if(AryaTools.Instance.Forms.RemarksForm != null)
                AryaTools.Instance.Forms.RemarksForm.Hide();

            var allExports =
                Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsSubclassOf(typeof(ExportWorker))).Select(
                    p => new
                             {
                                 Name = p.Name.Remove(0, 15).ToCamelCase(),
                                 p.FullName
                             }).ToList();

            allExports.Insert(0, new { Name = "-- Select Export --", FullName = string.Empty });
            allExports.Remove(new { Name = "Arya Xml", FullName = "Arya.Framework4.IO.Exports.ExportWorkerForAryaXml" });
            
            if (tscbExports.ComboBox != null)
            {
                tscbExports.ComboBox.DataSource = allExports;
                tscbExports.ComboBox.DisplayMember = "Name";
                tscbExports.ComboBox.ValueMember = "FullName";

                //resizes the combobox to show the longest string
                using (Graphics graphics = CreateGraphics())
                {
                    int maxWidth = 0;
                    foreach (object obj in tscbExports.ComboBox.Items)
                    {
                        var currentString = ((dynamic)obj).Name;
                        var area = graphics.MeasureString(currentString, tscbExports.ComboBox.Font);
                        maxWidth = Math.Max((int)area.Width, maxWidth);
                    }
                    tscbExports.Width = maxWidth;
                    tscbExports.ComboBox.Width = maxWidth + 15;
                }

            }

            pgExport.Text = @"Export File Options";
        }

        private void pgExport_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "IsSelected" && (bool)e.OldValue)
            {
                var currentWorker = (ExportWorker)(pgExport.SelectedObject);
                var removedTaxonomy = e.ChangedItem.Parent.Value as ExtendedTaxonomyInfo;
                if (removedTaxonomy != null)
                    currentWorker.Taxonomies.Remove(removedTaxonomy);
            }
        }

        private void tsbtnExport_Click(object sender, EventArgs e)
        {
            if (worker == null)
            {
                MessageBoxEx.Show(this,@"Please select an export", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var errors = worker.ValidateInput();
            if (errors.Count > 0)
            {
                string errorMessage = errors.Aggregate((a, b) => a + Environment.NewLine + b);

                MessageBoxEx.Show(this,errorMessage, @"Error(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (errors.Count == 0)
            {
                IsWorkerIdle = false;
                worker.ResetWorkerStatus();
                new Thread(worker.Run) { IsBackground = true }.Start();
                statusTimer.Start();
            }
        }

        private bool IsWorkerIdle
        {
            set
            {
                tsbtnExport.Enabled = value;
                pgExport.Enabled = value;
                tscbExports.Enabled = value;
                llSaveExportOptions.Enabled = value;
                llLoadOptions.Enabled = value;
            }
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            if (worker.CurrentProgress > worker.MaximumProgress)
                worker.MaximumProgress = worker.CurrentProgress + 1;

            pbExportProgress.Maximum = worker.MaximumProgress;
            if (!lblStatus.Text.Equals(worker.StatusMessage))
            {
                lblStatus.Text = worker.StatusMessage;
            }
            pbExportProgress.Value = worker.CurrentProgress;

            if (worker.State == Arya.Framework.Common.WorkerState.Working)
                return;

            var summary = worker.Summary;

            statusTimer.Stop();
            pbExportProgress.Value = pbExportProgress.Maximum;
            IsWorkerIdle = true;
        }

        private void tscbExports_SelectedIndexChanged(object sender, EventArgs e)
        {
            string exportClassName = ((dynamic)tscbExports.SelectedItem).FullName;

            if (string.IsNullOrEmpty(exportClassName))
            {
                worker = null;
                pgExport.SelectedObject = null;
                llSaveExportOptions.Enabled = false;
                return;
            }

            var d1 = Type.GetType(exportClassName);
            worker = (ExportWorker)Activator.CreateInstance(d1, new object[]{string.Empty, pgExport});
            
            if(taxonomyCollection != null)
            {
                worker.Taxonomies = taxonomyCollection;
            }
            else if(skuCollection != null)
            {
                worker.SkuCollection = skuCollection;
            }

            pgExport.Refresh();
            
            llSaveExportOptions.Enabled = worker.WorkerSupportsSaveOptions;
        }

        private const string PASSWORD = @"=_=iWOiH&S?3o|9|RA`\%=/-`"; //give strong password
        private const int ITERATIONS = 1024;
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes(@".EooY^N|T&'VN6h.\20(pCcVO");

        private void llSaveExportOptions_Click(object sender, EventArgs e)
        {
            if(pgExport.SelectedObject == null) return;

            string fileName;
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = @"Save Export Options";
                saveFileDialog.DefaultExt = ".eops";
                saveFileDialog.Filter = @"Export Options|*.eops|All files|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                }
                else return;
            }

            try
            {
                using (Stream s = File.Create(fileName))
                {
                    var keyBytes = new Rfc2898DeriveBytes(PASSWORD, Salt, ITERATIONS);
                    var rm = new RijndaelManaged { Key = keyBytes.GetBytes(32), IV = keyBytes.GetBytes(16) };
                    using (var cs = new CryptoStream(s, rm.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (var gs = new GZipStream(cs, CompressionMode.Compress))
                        {
                            var bf = new BinaryFormatter();
                            bf.Serialize(gs, pgExport.SelectedObject);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBoxEx.Show(this, "Unable to save export options", "Error", MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
            }
        }

        private void llLoadOptions_Click(object sender, EventArgs e)
        {
            string fileName;
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = @"Open Export Options";
                openFileDialog.DefaultExt = ".eops";
                openFileDialog.Filter = @"Export Options|*.eops|All files|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                }
                else return;
            }

            try
            {
                using (Stream s = File.OpenRead(fileName))
                {
                    var keyBytes = new Rfc2898DeriveBytes(PASSWORD, Salt, ITERATIONS);
                    var rm = new RijndaelManaged { Key = keyBytes.GetBytes(32), IV = keyBytes.GetBytes(16) };
                    using (var cs = new CryptoStream(s, rm.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (var gs = new GZipStream(cs, CompressionMode.Decompress))
                        {
                            var bf = new BinaryFormatter();
                            pgExport.SelectedObject = bf.Deserialize(gs);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBoxEx.Show(this, "Export options file is corrupted or invalid", "Invalid Export", MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
                return;
            }

            string className = ((dynamic)pgExport.SelectedObject.GetType()).FullName;

            bool found = false;
            foreach (var item in tscbExports.ComboBox.Items)
            {
                var itemClassName = ((dynamic)item).FullName;
                if (itemClassName != className) continue;
                tscbExports.SelectedIndexChanged -= tscbExports_SelectedIndexChanged;
                tscbExports.ComboBox.SelectedItem = item;
                tscbExports.SelectedIndexChanged += tscbExports_SelectedIndexChanged;
                found = true;
                ((ExportWorker)pgExport.SelectedObject).SetOwnerGrid(pgExport);
                worker = (ExportWorker)pgExport.SelectedObject;
                llSaveExportOptions.Enabled = worker.WorkerSupportsSaveOptions;
                break;
            }

            pgExport.Refresh();

            if (found) return;
            MessageBoxEx.Show(this, "Export does not exist", "Invalid Export", MessageBoxButtons.OK,
                              MessageBoxIcon.Exclamation);
            pgExport.SelectedObject = null;
            tscbExports.ComboBox.SelectedIndex = 0;
        }

    }
}
