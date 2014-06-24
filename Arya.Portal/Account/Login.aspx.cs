using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using Arya.Framework.Data.AryaDb;

namespace Arya.Portal.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void ValidateUser(object sender, EventArgs e)
        {
            using (var dc = new AryaDbDataContext())
            {
                var userInformation = (from user in dc.Users
                    where user.EmailAddress == Login1.UserName && user.OpenIdentity == Login1.Password
                    select user).FirstOrDefault();
                if (userInformation == null)
                    Login1.FailureText = "Invalid username or password";
                else
                {
                    Session["email"] = userInformation.EmailAddress;
                    Session["fullname"] = userInformation.FullName;

                    FormsAuthentication.RedirectFromLoginPage(Login1.UserName, Login1.RememberMeSet);
                }
            }
        }
    }
}
