using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Natalie.Portal
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private string _email;
        private string Email
        {
            get
            {
                if (_email == null)
                {
                    var response = Session["FetchResponse"] as FetchResponse;
                    _email = response.GetAttributeValue(WellKnownAttributes.Contact.Email) ?? "N/A";
                }
                return _email;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var response = Session["FetchResponse"] as FetchResponse;
            if (response == null)
            {
                //This should never be called!
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            string projectID = Request.QueryString["ProjectID"];

            if (Page.IsPostBack)
                return;

            GetDataFromTable();
            SaveValuesFromGrid();

        }

        protected void GetDataFromTable()
        {
            string connStr = ConfigurationManager.ConnectionStrings["importPortalConnString"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connStr);
            string selectStmt = "SELECT Model, Description, SubmittedBy, SubmittedOn, FileName, Status FROM NatalieImportRecord ORDER BY SubmittedOn DESC";
            SqlCommand command = new SqlCommand(selectStmt, sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            System.Data.DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);
            gv_status.DataSource = dt;
            gv_status.DataBind();
        }

        protected void SaveValuesFromGrid()
        {
            foreach (GridViewRow grdRow in gv_status.Rows)
            {
                var i = Int32.Parse(grdRow.RowIndex.ToString());
                var model = gv_status.Rows[i].Cells[0].Text;
                modelName.Value = model;
                var description = ((HyperLink)gv_status.Rows[i].Cells[1].Controls[0]).Text;
                desc.Value = description;
                var filename = gv_status.Rows[i].Cells[4].Text;
                fileName.Value = filename;
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            GetDataFromTable();
        }

        protected void btnNewImport_Click(object sender, EventArgs e)
        {
            Response.Redirect("Import.aspx");
        }

        protected void btnLaunchNatalie_Click(object sender, EventArgs e)
        {
            Response.Redirect("http://natalie.bytemanagers.com/Natalie4/Natalie4.application?ProjectID=projectID");
        }

        protected void imgDownload_Click(object sender, ImageClickEventArgs e)
        {
            // TODo - The list of rejected rows should be downloaded  so I can attempt to correct the problem and re-import. The Error log is an XML provided by Sabuj which has messages, ExceptionTypes and Stack Traces...
        }

        protected void imgLog_Click(object sender, ImageClickEventArgs e)
        {
            //ToDo - The results of the import and any problems encountered should be displayed in a new window..The Error log is an XML provided by Sabuj which has messages, ExceptionTypes and Stack Traces...
        }


    }
}