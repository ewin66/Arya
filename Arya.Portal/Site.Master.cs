using System;
using System.Web;
using System.Web.Security;

namespace Arya.Portal
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        public string Email
        {
            get
            {
                try
                {
                    return Session["email"].ToString();
                }
                catch (Exception ex)
                {
                    return "N/A";
                }
            }
        }
    }
}
