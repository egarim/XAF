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
using Microsoft.AspNetCore.Http.Extensions;
using XAF.Blazor.Server.Services.TokenSettings;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using System.Security.AccessControl;

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
            var test = context.Request.GetDisplayUrl();
            var _userPayloadService = context.RequestServices.GetRequiredService<IUserPayloadService>();
            var resultUser = _userPayloadService.GetUserDataPayload();

            if(_userPayloadService.GetUserDataPayload() == null)
            {
                await next(context);
            }
            else
            {
                string userId = resultUser.Oid;
                string userName = resultUser.Emails[0];
                Guid userOid = Guid.Empty;
                ApplicationUser myUser = null;
              
                if (configuration.GetConnectionString("ConnectionString") != null)
                {
                    XafTypesInfo.Instance.RegisterEntity(typeof(ApplicationUser));
                    using (XPObjectSpaceProvider directProvider = new XPObjectSpaceProvider(configuration.GetConnectionString("ConnectionString")))
                    using (IObjectSpace directObjectSpace = directProvider.CreateObjectSpace())
                    {
                        if (!resultUser.NewUser)
                        {
                            //myUser = directObjectSpace.CreateObject<ApplicationUser>();
                            //myUser.UserName = userName;
                            myUser = directObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == userName);
                            if (myUser == null)
                            {
                                myUser = directObjectSpace.CreateObject<ApplicationUser>();
                                myUser.UserName = userName;
                                // Set a password if the standard authentication type is used
                                myUser.SetPassword("asdasdasdasdasdasdasdasdasdasdas");

                                // The UserLoginInfo object requires a user object Id (Oid).
                                // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
                                directObjectSpace.CommitChanges(); //This line persists created object(s).
                                //((ISecurityUserWithLoginInfo)userAdmin).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, directObjectSpace.GetKeyValueAsString(userAdmin));
                            }
                            // If a role with the Administrators name doesn't exist in the database, create this role
                            PermissionPolicyRole adminRole = directObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
                            if (adminRole == null)
                            {
                                adminRole = directObjectSpace.CreateObject<PermissionPolicyRole>();
                                adminRole.Name = "Administrators";
                            }
                            adminRole.IsAdministrative = true;
                            myUser.Roles.Add(adminRole);
                            directObjectSpace.CommitChanges();
                        }
                        else
                        {
                            myUser = directObjectSpace.FindObject<ApplicationUser>(CriteriaOperator.Parse("UserName=?", userName));
                        }
                        var identityCreator = context.RequestServices.GetRequiredService<IStandardAuthenticationIdentityCreator>();
                        ClaimsIdentity id = identityCreator.CreateIdentity(myUser.Oid.ToString(), myUser.UserName);
                        await context.SignInAsync(new ClaimsPrincipal(id));
                        _userPayloadService.UserPayload= null;
                        context.Response.Redirect("/");
                    }
                }
            }


            


            /*string userId = context.Request.Query["UserID"];
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
            }*/


        }
    }
}
