using System;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using System.Web.Security;

namespace AryaPortal
{
    public partial class Profile : System.Web.UI.Page
    {
        public string Email = "N/A";
        public string FullName = "N/A";
        public string SsoId = "N/A";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            var response = Session["FetchResponse"] as FetchResponse;
            if (response == null)
            {
                //This should never be called!
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            // Retreive fields
            Email = response.GetAttributeValue(WellKnownAttributes.Contact.Email) ?? "N/A";
            FullName = GetFullname(
                    response.GetAttributeValue(WellKnownAttributes.Name.First), 
                    response.GetAttributeValue(WellKnownAttributes.Name.Last)) ?? "N/A";
            Session["FullName"] = FullName;
            SsoId = Session["ClaimedIdentifier"].ToString();
            //Country = response.GetAttributeValue(WellKnownAttributes.Contact.HomeAddress.Country) ?? "N/A";

            if (Email == "N/A")
            {
                FormsAuthentication.RedirectToLoginPage();
                //Response.Redirect(FormsAuthentication.LoginUrl + "?RETURNURL=~/Profile.aspx");
                return;
            }
          //  Server.Transfer("~/Import.aspx");
        }

        private static string GetFullname(string first, string last)
        {
            var _first = first ?? "";
            var _last = last ?? "";

            if (string.IsNullOrEmpty(_first) || string.IsNullOrEmpty(_last))
                return "";

            return _first + " " + _last;
        }
    }

}