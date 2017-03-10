using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using GMSPPS.Models;


namespace GMSPPS.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        //public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        //{
        //    //wenn hier web und selv nicht gleich sind gets ins nirvana
        //    if (context.ClientId == _publicClientId)
        //    {
        //        Uri expectedRootUri = new Uri(context.Request.Uri, "/");

        //        if (expectedRootUri.AbsoluteUri == context.RedirectUri)
        //        {
        //            context.Validated();
        //        }
        //    }

        //    return Task.FromResult<object>(null);
        //}


        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
                else if (context.ClientId == "web")
                {
                    var expectedUri = new Uri(context.Request.Uri, "/");
                    context.Validated(expectedUri.AbsoluteUri);
                }
            }
            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        //public class MyOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
        //{
        //    public override async Task ValidateClientAuthentication(
        //        OAuthValidateClientAuthenticationContext context)
        //    {
        //        string clientId;
        //        string clientSecret;

        //        if (context.TryGetBasicCredentials(out clientId, out clientSecret))
        //        {
        //            UserManager<IdentityUser> userManager =
        //                context.OwinContext.GetUserManager<UserManager<IdentityUser>>();
        //            OAuthDbContext dbContext =
        //                context.OwinContext.Get<OAuthDbContext>();

        //            try
        //            {
        //                Client client = await dbContext
        //                    .Clients
        //                    .FirstOrDefaultAsync(clientEntity => clientEntity.Id == clientId);

        //                if (client != null &&
        //                    userManager.PasswordHasher.VerifyHashedPassword(
        //                        client.ClientSecretHash, clientSecret) == PasswordVerificationResult.Success)
        //                {
        //                    // Client has been verified.
        //                    context.OwinContext.Set<Client>("oauth:client", client);
        //                    context.Validated(clientId);
        //                }
        //                else
        //                {
        //                    // Client could not be validated.
        //                    context.SetError("invalid_client", "Client credentials are invalid.");
        //                    context.Rejected();
        //                }
        //            }
        //            catch
        //            {
        //                // Could not get the client through the IClientManager implementation.
        //                context.SetError("server_error");
        //                context.Rejected();
        //            }
        //        }
        //        else
        //        {
        //            // The client credentials could not be retrieved.
        //            context.SetError(
        //                "invalid_client",
        //                "Client credentials could not be retrieved through the Authorization header.");

        //            context.Rejected();
        //        }
        //    }

        //    public override async Task GrantResourceOwnerCredentials(
        //        OAuthGrantResourceOwnerCredentialsContext context)
        //    {
        //        Client client = context.OwinContext.Get<Client>("oauth:client");
        //        if (client.AllowedGrant == OAuthGrant.ResourceOwner)
        //        {
        //            // Client flow matches the requested flow. Continue...
        //            UserManager<IdentityUser> userManager =
        //                context.OwinContext.GetUserManager<UserManager<IdentityUser>>();

        //            IdentityUser user;
        //            try
        //            {
        //                user = await userManager.FindAsync(context.UserName, context.Password);
        //            }
        //            catch
        //            {
        //                // Could not retrieve the user.
        //                context.SetError("server_error");
        //                context.Rejected();

        //                // Return here so that we don't process further. Not ideal but needed to be done here.
        //                return;
        //            }

        //            if (user != null)
        //            {
        //                try
        //                {
        //                    // User is found. Signal this by calling context.Validated
        //                    ClaimsIdentity identity = await userManager.CreateIdentityAsync(
        //                        user,
        //                        DefaultAuthenticationTypes.ExternalBearer);

        //                    context.Validated(identity);
        //                }
        //                catch
        //                {
        //                    // The ClaimsIdentity could not be created by the UserManager.
        //                    context.SetError("server_error");
        //                    context.Rejected();
        //                }
        //            }
        //            else
        //            {
        //                // The resource owner credentials are invalid or resource owner does not exist.
        //                context.SetError(
        //                    "access_denied",
        //                    "The resource owner credentials are invalid or resource owner does not exist.");

        //                context.Rejected();
        //            }
        //        }
        //        else
        //        {
        //            // Client is not allowed for the 'Resource Owner Password Credentials Grant'.
        //            context.SetError(
        //                "invalid_grant",
        //                "Client is not allowed for the 'Resource Owner Password Credentials Grant'");

        //            context.Rejected();
        //        }
        //    }
        //}

        //public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{
        //    var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

        //    ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

        //    if (user == null)
        //    {
        //        context.SetError("invalid_grant", "The user name or password is incorrect.");
        //        return;
        //    }

        //    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
        //       OAuthDefaults.AuthenticationType);
        //    ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
        //        CookieAuthenticationDefaults.AuthenticationType);

        //    AuthenticationProperties properties = CreateProperties(user.UserName);
        //    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
        //    context.Validated(ticket);
        //    context.Request.Context.Authentication.SignIn(cookiesIdentity);
        //}

        //public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        //{
        //    foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
        //    {
        //        context.AdditionalResponseParameters.Add(property.Key, property.Value);
        //    }

        //    return Task.FromResult<object>(null);
        //}

        //public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        //{
        //    if (context.ClientId == _publicClientId)
        //    {
        //        Uri expectedRootUri = new Uri(context.Request.Uri, "/");

        //        if (expectedRootUri.AbsoluteUri == context.RedirectUri)
        //        {
        //            context.Validated();
        //        }
        //        else if (context.ClientId == "web")
        //        {
        //            var expectedUri = new Uri(context.Request.Uri, "/");
        //            context.Validated(expectedUri.AbsoluteUri);
        //        }
        //    }

        //    return Task.FromResult<object>(null);
        //}
        //public static AuthenticationProperties CreateProperties(string userName)
        //{
        //    IDictionary<string, string> data = new Dictionary<string, string>
        //    {
        //        { "userName", userName }
        //    };
        //    return new AuthenticationProperties(data);
        //}
    }
}