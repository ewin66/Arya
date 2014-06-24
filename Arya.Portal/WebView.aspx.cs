using System;
using System.Web.Security;
using System.Web.UI;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

namespace Arya.Portal
{
    public partial class WebView : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var response = Session["FetchResponse"] as FetchResponse;
            if (response == null)
            {
                //This should never be called!
                FormsAuthentication.RedirectToLoginPage();
                return;
            }

            const string webViewUrl = "http://dev.empirisense.com/AryaWebView/Default.aspx";

            var username = response.GetAttributeValue(WellKnownAttributes.Contact.Email);
            var password = Session["ClaimedIdentifier"].ToString();

            Response.Redirect(string.Format("{0}?txtUsername={1}&txtPassword={2}", webViewUrl, username, password));

//            Response.Clear();

//            var html = string.Format(@"
//                <html>
//                    <body onload='document.forms[""form1""].submit()'>
//                        <form name='form1' action='{0}' method='post'>
//                        <input type='hidden' name='txtUsername' value='{1}'>
//                        <input type='hidden' name='txtPassword' value='{2}'>
//                        Redirecting...
//                    </form>
//                    </body>
//                </html>", webViewUrl, username, password);

//            Response.Write(html);

//            Response.End();
        }
    }
}