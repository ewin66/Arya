using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using Natalie.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Natalie.Framework.IO.Exports;

namespace Natalie.Portal
{
    public partial class ExportTree : System.Web.UI.Page
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
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            if (!Page.IsPostBack)
            {
                DisplayProjectList();
                // GetAllExportTypes();
                GetAllSourceTypes();
            }
               
        }

         protected void DisplayProjectList()
        {
            var availableProjects = new List<ListItem>();
            using (var Dc = new NatalieDbDataContext())
            {
                var currUser = (from u in Dc.Users
                                where u.EmailAddress == Email
                                select u).FirstOrDefault();

                userproject["UserID"] = currUser.ID.ToString();

                var userName = currUser.FullName;
                var userRole = currUser.IsAdmin;

                var existingDatabases = Dc.ExecuteQuery<string>("select name from sys.databases").ToList();

                IEnumerable<UserProject> up = currUser.UserProjects;
                IEnumerable<Project> p = Dc.Projects;
                if (!userRole)
                {
                    up = up.Where(dp => dp.GroupID == Group.ExportAdminGroup || dp.GroupID == Group.ExportUserGroup
                        && existingDatabases.Contains(dp.Project.DatabaseName))
                        .Select(dp => dp);
                    availableProjects = up.Select(dp => new ListItem { Text = dp.Project.ClientDescription + " " + dp.Project.SetName }).Distinct().ToList();
                }

                else
                {
                    p = p.Where(dp => existingDatabases.Contains(dp.DatabaseName))
                           .Select(dp => dp);
                    availableProjects = p.Select(dp => new ListItem { Text = dp.ClientDescription + " " + dp.SetName }).Distinct().ToList();
                }

                ddlProject.Items.Insert(0, new DevExpress.Web.ASPxEditors.ListEditItem { Text = "--Select a Model--", Value= string.Empty });
                foreach (ListItem ap in availableProjects)
                {
                    ddlProject.Items.Add(new DevExpress.Web.ASPxEditors.ListEditItem { Text = ap.ToString(), Value = ap.ToString() });
                }
               
            }
    
        }

        protected void GetAllSourceTypes()
        {
            var itemNames = Enum.GetNames(typeof(Natalie.Framework.IO.Exports.ExportWorkerBase.SourceType));
            ddlSourceType.Items.Insert(0, new DevExpress.Web.ASPxEditors.ListEditItem { Text = "--Select an option--", Value = string.Empty });
            foreach (var item in itemNames)
            {
                ddlSourceType.Items.Add(new DevExpress.Web.ASPxEditors.ListEditItem { Text = item.ToString(), Value = item.ToString() });
            }
        }
  
        //protected void GetAllExportTypes()
        //{
        //    var allExports = Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsSubclassOf(typeof(ExportWorkerBase)))
        //        .Select(p => new
        //          {
        //              Name = p.Name.Remove(0, 15),
        //              p.FullName
        //          }).ToList();

        //    ddlExportType.Items.Insert(0, new DevExpress.Web.ASPxEditors.ListEditItem{ Text = "-- Select Export --", Value = string.Empty });
        //    foreach (var exp in allExports)
        //    {
        //        ddlExportType.Items.Add(new DevExpress.Web.ASPxEditors.ListEditItem { Text = exp.ToString(), Value = exp.ToString() });
        //    }
        //}

        protected void ddlProject_Validation(object sender, DevExpress.Web.ASPxEditors.ValidationEventArgs e)
        {
            
        }

        protected void txtDescription_Validation(object sender, DevExpress.Web.ASPxEditors.ValidationEventArgs e)
        {

        }

        //protected void ddlExportType_Validation(object sender, DevExpress.Web.ASPxEditors.ValidationEventArgs e)
        //{

        //}

        protected void ddlSourceType_Validation(object sender, DevExpress.Web.ASPxEditors.ValidationEventArgs e)
        {

        }

        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentProject = ddlProject.Text.ToString();
            using (var Dc = new NatalieDbDataContext())
            {
                var currentProjectID = (from pr in Dc.Projects
                                        where pr.ClientDescription + ' ' + pr.SetName == currentProject.ToString()
                                        select pr.ID).FirstOrDefault();
                userproject["ProjectID"] = currentProjectID.ToString();
            }
          
        }

        protected void ddlSourceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentSourceType = ddlSourceType.Text.ToString();
            if (currentSourceType == "Taxonomy")
            {
               lnk_addTax.Visible = true;
            }
            else if (currentSourceType == "SkuList")
            {
                lnk_skuList.Visible = true;
            }
         
        }

        protected void lnk_addTax_Click(object sender, EventArgs e)
        {

        }

        protected void lnk_skuList_Click(object sender, EventArgs e)
        {
           //var tree = new TaxonomyTree(Guid.Parse(userproject["ProjectID"].ToString()), Guid.Parse(userproject["UserID"].ToString()));
           //panel1.Controls.Add(tree);
        }

     }
 }
