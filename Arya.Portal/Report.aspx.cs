using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using Arya.Framework.Common;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;
using Arya.Framework.IO.Imports;
using Polenter.Serialization;
using System.Web.UI.WebControls;

namespace Arya.Portal
{
    public partial class Report : Page
    {
        private readonly string connStr =
            Arya.Framework.Utility.Util.GetAryaServicesConnectionString();

        private Dictionary<string, int> dict = new Dictionary<string, int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var sqlConnection = new SqlConnection(connStr);
            //string submittedBy = Session["SubmittedBy"].ToString();
            var projectID = Request.QueryString["ProjectID"];
            using (var Dc = new AryaDbDataContext())
            {
                var projectName = (from p in Dc.Projects
                                   where p.ID == Guid.Parse(projectID)
                                   select p.ClientDescription + ' ' + p.SetName).Single();
                txtProject.Text = projectName;
            }
            var id = Request.QueryString["ID"];
            var description = Request.QueryString["Description"];
            txtProjectDesc.Text = description;
            var basepath = ConfigurationManager.AppSettings["BasePath"];
            var cmd = new SqlCommand();
            cmd.CommandText = "Select ArgumentDirectoryPath from AryaTask Where ID= @id";
            cmd.Parameters.AddWithValue("@ID", Guid.Parse(id));
            cmd.Connection = sqlConnection;
            sqlConnection.Open();
            var xmlpathValue = cmd.ExecuteScalar().ToString();
            var path = Path.Combine(basepath, xmlpathValue);
            var ArgumentDirectoryPath = Path.Combine(path, WorkerArguments.ArgumentsFileName);
            ReadDataFromXMLFile(ArgumentDirectoryPath);
            var logPath = Path.Combine(path, "log.xml");
            HyperLink hlink_log = new HyperLink();
            hlink_log.Text = "View log";
            hlink_log.NavigateUrl = "Log.aspx?ID="+id;
            phLog.Controls.Add(hlink_log);
            sqlConnection.Close();

            var cmd1 = new SqlCommand();
            cmd1.CommandText = "Select SubmittedBy from AryaTask Where ID= @id";
            cmd1.Parameters.AddWithValue("@ID", Guid.Parse(id));
            cmd1.Connection = sqlConnection;
            sqlConnection.Open();
            var submittedBy = cmd1.ExecuteScalar().ToString();
            using (var Dc = new AryaDbDataContext())
            {
                var userName = (from u in Dc.Users
                                where u.ID == Guid.Parse(submittedBy)
                                   select u.FullName).Single();
                txtSubmittedBy.Text = userName;
            }
            sqlConnection.Close();

            var cmd2 = new SqlCommand();
            cmd2.CommandText = "Select SubmittedOn from AryaTask Where ID= @id";
            cmd2.Parameters.AddWithValue("@ID", Guid.Parse(id));
            cmd2.Connection = sqlConnection;
            sqlConnection.Open();
            var submittedOn = cmd2.ExecuteScalar().ToString();
            txtSubmittedOn.Text = submittedOn;
            sqlConnection.Close();        
        }

        protected void ReadDataFromXMLFile(string fileName)
        {
            var serializer = new SharpSerializer();
            var importArgs = new ImportArgs();
            importArgs = (ImportArgs) serializer.Deserialize(fileName);
            var fieldDelimiter = importArgs.FieldDelimiter.ToString();
            if (fieldDelimiter != null)
                txtDelimiter.Text = fieldDelimiter;
            var importOptions = importArgs.CurrentImportOptions.ToString();
            var io = importOptions.Split(new[] {','}, StringSplitOptions.None);
            foreach (var i in io)
            {
                switch (i.Trim())
                {
                    case "None":
                        txtimportSelected.Text += "None" + " ";
                        break;

                    case "CreateMissingSkus":
                        txtimportSelected.Text += "CreateMissingSkus" + " ";
                        break;

                    case "CreateMissingTaxonomies":
                        txtimportSelected.Text += "CreateMissingTaxonomies" + " ";
                        break;

                    case "CreateMissingAttributes":
                        txtimportSelected.Text += "CreateMissingAttributes" + " ";
                        break;

                    case "CreateMissingMetaAttributes":
                        txtimportSelected.Text += "CreateMissingMetaAttributes" + " ";
                        break;

                    case "CreateMissingValues":
                        txtimportSelected.Text += "CreateMissingValues" + " ";
                        break;

                }
            }
            dict = importArgs.FieldMappings;
            gv_mapping.DataSource = (from dictitem in dict
                                     select
                                         new
                                             {
                                                 HeaderColumn = "Column" + ' ' + dictitem.Value,
                                                 MappingFields = dictitem.Key
                                             });
            gv_mapping.DataBind();

            var filename = importArgs.InputFilePath;
            var file = filename.Substring(filename.LastIndexOf('\\') + 1);
            txtinputfileName.Text = file;
        }
    }
}