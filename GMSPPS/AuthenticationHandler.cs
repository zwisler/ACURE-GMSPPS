using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Security.Principal;
using System.Net;
using System.Web;
using Citaurus.WebClientTool;
using System.Diagnostics;

namespace GMSPPS
{
    public class AuthenticationHandler : DelegatingHandler
    {
        public GMSPPSEntities ctx;
        protected override Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                Trace.TraceInformation("AuthenticationHandler wird aufgerufen");
                if ( request.Headers.Authorization != null)
                { 
                    var authorizationHeader = request.Headers.GetValues("Authorization").First();
                    //Login is from Google (Androd Device)

                    if (authorizationHeader != null && authorizationHeader
                        .StartsWith("GMSPPSg ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Trace.TraceInformation("Google Tooken erkannt");
                        ctx = new GMSPPSEntities();
                        string authorizationUserAndPwdBase64 =
                            authorizationHeader.Substring("GMSPPSg ".Length);
                        Trace.TraceInformation($"Google Tooken: {authorizationUserAndPwdBase64}");
                        //string authorizationUserAndPwd = Encoding.Default
                        //  .GetString(Convert.FromBase64String(authorizationUserAndPwdBase64));
                        GoggleToken token = new GoggleToken();
                        dynamic ret = token.GetTokeninfo(authorizationUserAndPwdBase64);
                        string mail = ret.email;
                        string name = ret.name;
                        string GoggleID = ret.sub;
                        string user = name;
                        string password = ret.aud;
                       // if (mail != null && name == null) name = mail;
                        if (verifyUserAndPwd(password))
                        {
                            GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.GPMS_DEVICE_TYPE_ID == 4 && x.Email == mail).FirstOrDefault();
                            if (Client == null)
                            {
                                //client eintragen
                                GPMS_CLIENT newClient = new GPMS_CLIENT()
                                {
                                    Email = mail,
                                    UserName = name,
                                    GPMS_DEVICE_TYPE_ID = 4,
                                    GoogleID = GoggleID
                                };
                                ctx.GPMS_CLIENT.Add(newClient);
                                ctx.SaveChanges();
                            }

                            // Attach the new principal object to the current HttpContext object
                            HttpContext.Current.User =
                                new GenericPrincipal(new GenericIdentity(mail), new string[0]);
                            System.Threading.Thread.CurrentPrincipal =
                                System.Web.HttpContext.Current.User;
                        }
                        //Googe Token Prüfung Negativ
                        else return Unauthorized();
                    }
                    else if (authorizationHeader != null && authorizationHeader
                   .StartsWith("WTooken ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Trace.TraceInformation("Windows Live Tooken erkannt");
                        ctx = new GMSPPSEntities();
                        string authorizationUserAndPwdBase64 =
                            authorizationHeader.Substring("WTooken ".Length);
                        //string authorizationUserAndPwd = Encoding.Default
                        //  .GetString(Convert.FromBase64String(authorizationUserAndPwdBase64));
                        MicrosoftToken token = new MicrosoftToken();
                        dynamic ret = token.GetTokeninfo(authorizationUserAndPwdBase64);
                        dynamic emails = ret.emails;
                        string mail = emails.preferred;
                        string name = ret.name;
                        string LiveID = ret.id;
                        Trace.TraceInformation("Windows Live ID " + LiveID + " name: " + name + "email: " + mail);
                        if (verifyWindowsUser(ret))
                        {
                            Trace.TraceInformation("Windows Live Tooken gültig");
                            GPMS_CLIENT Client = ctx.GPMS_CLIENT.Where(x => x.GPMS_DEVICE_TYPE_ID == 1 && x.Email == mail).FirstOrDefault();
                            if (Client == null)
                            {
                                //client eintragen
                                GPMS_CLIENT newClient = new GPMS_CLIENT()
                                {
                                    Email = mail,
                                    UserName = name,
                                    GPMS_DEVICE_TYPE_ID = 1,
                                    LiveID = LiveID
                                };
                                ctx.GPMS_CLIENT.Add(newClient);
                                ctx.SaveChanges();
                            }

                            // Attach the new principal object to the current HttpContext object
                            HttpContext.Current.User =
                                new GenericPrincipal(new GenericIdentity(mail), new string[0]);
                            System.Threading.Thread.CurrentPrincipal =
                                System.Web.HttpContext.Current.User;
                        }
                        //Microsoft Token Prüfung Negativ
                        else return Unauthorized();
                    }
                    else if (authorizationHeader != null && authorizationHeader
                  .StartsWith("GMSPPS_Tooken ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //GMSPPS Tooken a Povider
                        Trace.TraceInformation("GMSPPS Provider is Callin Web API");
                        ctx = new GMSPPSEntities();
                        string authorizationUserAndPwdBase64 =
                            authorizationHeader.Substring("GMSPPS_Tooken ".Length);
                        GPMS_PROVIDER Provider = ctx.GPMS_PROVIDER.Where(x => x.API_TOOKEN == authorizationUserAndPwdBase64).FirstOrDefault();
                        if (Provider != null)
                        {
                            GenericIdentity MyIdentity = new GenericIdentity(Provider.Name);
                            // Attach the new principal object to the current HttpContext object
                            // Create generic principal.
                            String[] MyStringArray = { "GMSPPS_Provider", $"PROVIDER_TYP_{Provider.GPMS_PROVIDER_TYP_ID.ToString()}" };
                            GenericPrincipal MyPrincipal =
                                new GenericPrincipal(MyIdentity, MyStringArray);

                            HttpContext.Current.User = MyPrincipal;
                                
                            System.Threading.Thread.CurrentPrincipal =
                                System.Web.HttpContext.Current.User;
                        }
                        //Provider Token Prüfung Negativ
                        else return Unauthorized();

                    }

                    else return Unauthorized();
            }


            }
            catch (WebException e)
            {
                Trace.TraceError("AuthenticationHandler Fehler:" + e);
                 return Unauthorized();
            }

            return base.SendAsync(request, cancellationToken);
        }

        private bool verifyUserAndPwd(string password)
        {
            Trace.TraceInformation("AuthenticationHandler GOOGLE" + password);
            // aud stimmt.
            //return password == "232925292391-pn2oegkpd7qq1d3bse5d5qsherrphhsk.apps.googleusercontent.com";
            return password == "414999757757-t0i02p4g2cjlnfpu6bm1valmrj7csfec.apps.googleusercontent.com";
        }
        private bool verifyWindowsUser(dynamic MicrosoftResult)
        {
            // TODO Microsort Reqest prüfen.
            return ((string)MicrosoftResult.id).Length > 1;                
        }

        private Task<HttpResponseMessage> Unauthorized()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }
    }
}