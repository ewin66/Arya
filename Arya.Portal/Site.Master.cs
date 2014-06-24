using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

namespace Arya.Portal
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private string _email;

        public string Email
        {
            get
            {
                if (_email != null)
                    return _email;

                var response = Session["FetchResponse"] as FetchResponse;
                if (response == null)
                    return null;

                _email = response.GetAttributeValue(WellKnownAttributes.Contact.Email) ?? "N/A";
                return _email;
            }
        }
    }
}
