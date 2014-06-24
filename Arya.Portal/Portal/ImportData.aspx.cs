using System;
using System.IO;
using System.Linq;
using LinqKit;
using Arya.Portal.SupportingClasses;
using AryaPortal.SupportingClasses;

namespace Arya.Portal.Portal
{
    public partial class ImportData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SelectFormat();
        }

        protected void btnInputFile_Click(object sender, EventArgs e)
        {
            var fileName = UploadInputFile();

            if (fileName == null)
                return;

            InputFileName = fileName;

            GetTables();
        }

        private string InputFileName
        {
            get
            {
                return Session["InputFileName"].ToString();
            }
            set
            {
                Session["InputFileName"] = value;
            }
        }

        private InputProcessor InitProcessor()
        {
            var inputFile = new FileInfo(InputFileName);

            InputProcessor processor = null;
            switch (inputFile.Extension)
            {
                case ".xls":
                case ".xlsx":
                    processor = new ExcelProcessor(inputFile);
                    break;

                case ".csv":
                case ".txt":
                    processor = new TextProcessor(inputFile);
                    break;

                default:
                    return null;
            }

            return processor;
        }

        private void GetTables()
        {
            var processor = InitProcessor();

            if (processor == null)
            {
                lblInputFile.Text = "Unknown file type.";
                return;
            }

            lbTables.Items.Clear();
            processor.GetTables().ForEach(lbTables.Items.Add);

            if(lbTables.Items.Count==0)
            {
                lblInputFile.Text = "No data found.";
                return;
            }

            lbTables.Items[0].Selected = true;
            SelectTable(lbTables.Items[0].Value);
        }

        private string UploadInputFile()
        {
            if (fuInputFile.HasFile)
            {
                try
                {
                    var tmp = Guid.NewGuid();
                    var ext = Path.GetExtension(fuInputFile.FileName);

                    var filename = Server.MapPath("~/Temp/" + tmp + ext);
                    fuInputFile.SaveAs(filename);
                    lblInputFile.Text = "Uploaded";

                    return filename;
                }
                catch (Exception ex)
                {
                    lblInputFile.Text = ex.Message;
                }
            }

            return null;
        }

        protected void lbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectTable(lbTables.SelectedValue);
        }

        private void SelectTable(string tableName)
        {
            var processor = InitProcessor();

            GetSampleData(processor, tableName);

            PopulateMappingColumns(processor, tableName);
        }

        private void PopulateMappingColumns(InputProcessor processor, string tableName)
        {
            var columns = processor.GetHeaderColumns(tableName).Select(col => new {ColumnName = col});
            gvMapping.DataSource = columns;
            gvMapping.DataBind();
        }

        private void GetSampleData(InputProcessor processor, string tableName)
        {
            var results = processor.GetSampleData(tableName);
            gvDataSample.DataSource = results;
            gvDataSample.DataBind();
        }

        protected void ddFileFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectFormat();
        }

        private void SelectFormat()
        {
            switch (ddFileFormat.SelectedValue.Substring(0,5).ToLower())
            {
                case "horiz":
                    imgSampleImage.ImageUrl = "~/images/HorizontalFormat.png";
                    break;

                case "verti":
                    imgSampleImage.ImageUrl = "~/images/VerticalFormat.png";
                    break;

                default:
                    imgSampleImage.ImageUrl = string.Empty;
                    break;
            }
        }
    }
}