using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Natalie.Portal
{
    public partial class XmlTransform : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string description = Request.QueryString["Description"];
            string model = Request.QueryString["Model"];
            string filename = Request.QueryString["FileName"];
            //string connStr = ConfigurationManager.ConnectionStrings["importPortalConnString"].ConnectionString;
            //SqlConnection sqlConnection = new SqlConnection(connStr);
            //System.Text.StringBuilder builder = new System.Text.StringBuilder();
            //string selectStmt = "SELECT ImportArgs FROM ImportArgsRecord WHERE FileName = filename";
            //SqlCommand command = new SqlCommand(selectStmt, sqlConnection);
            //sqlConnection.Open();
            //SqlDataReader reader = command.ExecuteReader();
            var report = new Report();
            Serialize(report);
            //sqlConnection.Close();

        }

        public string Serialize(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());

            var writerSettings =
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true
                };

            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);

            var stringWriter = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
            {
                serializer.Serialize(xmlWriter, obj, emptyNameSpace);

                return stringWriter.ToString();
            }
        }

        }
    }
