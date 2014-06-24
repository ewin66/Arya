using System;
using System.Net;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace AryaPortal.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                // Handles relying party's request
                HandleRelyingPartyRequest();
            }
            else
            {
                // Handles a redirect from OpenID Provider
                // Handles a first time load of this page
                HandleOpneIdProviderResponse();
            }
        }


        private void HandleOpneIdProviderResponse()
        {
            // Define a new instance of OpenIdRelyingParty class
            using (var openid = new OpenIdRelyingParty())
            {
                // Get authentication response from OpenId Provider Create IAuthenticationResponse instance to be used
                // to retreive the response from OP
                var response = openid.GetResponse();

                // No authentication request was sent
                if (response == null) return;

                switch (response.Status)
                {
                    // If user was authenticated
                    case AuthenticationStatus.Authenticated:
                        // This is where you would look for any OpenID extension responses included
                        // in the authentication assertion.
                        var fetchResponse = response.GetExtension<FetchResponse>();

                        // Store the "Queried Fields"
                        Session["FetchResponse"] = fetchResponse;
                        Session["ClaimedIdentifier"] = response.ClaimedIdentifier;

                        // Use FormsAuthentication to tell ASP.NET that the user is now logged in,
                        // with the OpenID Claimed Identifier as their username.
                        FormsAuthentication.RedirectFromLoginPage(response.ClaimedIdentifier, false);
                        break;
                    // User has cancelled the OpenID Dance
                    case AuthenticationStatus.Canceled:
                        this.loginCanceledLabel.Visible = true;
                        break;
                    // Authentication failed
                    case AuthenticationStatus.Failed:
                        this.loginFailedLabel.Visible = true;
                        break;
                }

            }
        }


        private void HandleRelyingPartyRequest()
        {
            try
            {
                // Create a new instance of OpenIdRelyingParty
                using (var openid = new OpenIdRelyingParty())
                {
                    // Create IAuthenticationRequest instance representing the Relying Party sending an authentication request
                    var request = openid.CreateRequest(Request.Form["openid_identifier"]);

                    // This is where you would add any OpenID extensions you wanted
                    // to include in the authentication request. In this case, we are making use of OpenID Attribute Exchange 1.0
                    // to fetch additional data fields from the OpenID Provider 
                    var fetchRequest = new FetchRequest();
                    fetchRequest.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                    fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.First);
                    fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.Last);
                    request.AddExtension(fetchRequest);

                    // Issue request to OP
                    request.RedirectToProvider();
                }
            }
            catch (ProtocolException pExp)
            {
                // The user probably entered an Identifier that was not a valid OpenID endpoint.
            }
            catch (WebException ex)
            {
                // The user probably entered an Identifier that was not a valid OpenID endpoint.
            }
        }
    }
}
