using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XAF.Module.BusinessObjects;
using DevExpress.ExpressApp.Security.Authentication.Internal;
using System.Text;

namespace XAF.Blazor.Server
{
    public class AutoSignInMiddleware
    {
        private readonly RequestDelegate next;
        private IConfiguration configuration;
        public AutoSignInMiddleware(IConfiguration config, RequestDelegate next)
        {
            configuration = config;
            this.next = next;
        }
        private static async Task SignIn(HttpContext context, PermissionPolicyUser user, string userName)
        {
                     

            var identityCreator = context.RequestServices.GetRequiredService<IStandardAuthenticationIdentityCreator>();
            ClaimsIdentity id = identityCreator.CreateIdentity(user.Oid.ToString(),user.UserName);
            await context.SignInAsync(new ClaimsPrincipal(id));
            context.Response.Redirect("/");



            //ClaimsIdentity id = new ClaimsIdentity(SecurityDefaults.Issuer);
            //Claim claim = new Claim(ClaimTypes.NameIdentifier, user.Oid.ToString(), ClaimValueTypes.String, SecurityDefaults.Issuer);
            //id.AddClaim(claim);
            //await context.SignInAsync(new ClaimsPrincipal(id));
            //context.Response.Redirect("/");
        }
        public async Task Invoke(HttpContext context)
        {

            var request = context.Request;

            

            // Get the URL from the HttpRequest object
            string url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";


            // Checking whether this is a callback request from Azure B2C
            if (request.Path.StartsWithSegments(new PathString("/your-callback-endpoint")))
            {
                if (request.Method == HttpMethods.Get)
                {
                    // For GET requests (e.g., in OAuth2 Authorization Code flow)
                    var authorizationCode = request.Query["code"].ToString();
                    var state = request.Query["state"].ToString();

                    // Do something with the authorizationCode and state
                }
                else if (request.Method == HttpMethods.Post)
                {
                    // For POST requests, you would read from the form body
                    if (request.HasFormContentType)
                    {
                        var form = await request.ReadFormAsync();
                        var someValue = form["someKey"];

                        // Do something with the form data
                    }
                }

                // You can also inspect headers, cookies, etc.
            }

            string userId = context.Request.Query["UserID"];
            Guid userOid = Guid.Empty;
            ApplicationUser myUser = null;
            if (Guid.TryParse(userId, out userOid))
            {
                if (!(context.User?.Identity.IsAuthenticated ?? false) && !string.IsNullOrEmpty(userId))
                {
                    bool autoLoginOK = false;
                    if (configuration.GetConnectionString("ConnectionString") != null)
                    {
                        using (XPObjectSpaceProvider directProvider = new XPObjectSpaceProvider(configuration.GetConnectionString("ConnectionString")))
                        using (IObjectSpace directObjectSpace = directProvider.CreateObjectSpace())
                        {
                             myUser = directObjectSpace.FindObject<ApplicationUser>(CriteriaOperator.Parse("Oid=?", userOid));
                            if (myUser != null)
                                if (myUser.AutoLoginByURL) 
                                {
                                    autoLoginOK = true;
                                }
                        }
                    }

                    if (autoLoginOK)
                    {


                        var identityCreator = context.RequestServices.GetRequiredService<IStandardAuthenticationIdentityCreator>();
                        ClaimsIdentity id = identityCreator.CreateIdentity(myUser.Oid.ToString(), myUser.UserName);
                        await context.SignInAsync(new ClaimsPrincipal(id));
                        context.Response.Redirect("/");

                        //ClaimsIdentity id = new ClaimsIdentity(SecurityDefaults.DefaultClaimsIssuer);
                        //Claim claim = new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String, SecurityDefaults.Issuer);
                        //id.AddClaim(claim);
                        //await context.SignInAsync(new ClaimsPrincipal(id));
                        //context.Response.Redirect("/");
                    }
                    else
                        await next(context);
                }
                else
                    await next(context);
            }
            else
            {
                await next(context);
            }
        }
    }
}
