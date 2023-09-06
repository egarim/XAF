using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAF.Module.Controllers
{
    public class RedirectController : ViewController
    {
        //https://docs.devexpress.com/eXpressAppFramework/404402/application-shell-and-base-infrastructure/dependency-injection-in-xaf-applications/dependency-injection-in-controllers?utm_source=SupportCenter&utm_medium=website&utm_campaign=docs-feedback&utm_content=T1159264
        private readonly IServiceProvider serviceProvider;  
        public RedirectController() : base()
        {
            // Target required Views (use the TargetXXX properties) and create their Actions.
            
        }
        // Implement this constructor to support dependency injection.
        //[ActivatorUtilitiesConstructor]
        //public RedirectController(IServiceProvider serviceProvider) : this()
        //{
        //    this.serviceProvider = serviceProvider;
        //}
        protected async override void OnActivated()
        {
            base.OnActivated();
            var Js = (this.Application as BlazorApplication).ServiceProvider.GetRequiredService<IJSRuntime>();//this.serviceProvider.GetRequiredService<IJSRuntime>();
            //await Js.InvokeVoidAsync("redirect", "https://efilec2cdev.b2clogin.com/efilec2cdev.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_SignUpSignIn&client_id=b770840b-2047-46c6-8d55-09aa23cb6bbe&nonce=defaultNonce&redirect_uri=https%3A%2F%2Flocalhost%3A44318%2Ftokenreceiver&scope=openid&response_type=id_token&prompt=login");
            //var Nav=this.serviceProvider.GetRequiredService<NavigationManager>();
            //Nav.NavigateTo("https://efilec2cdev.b2clogin.com/efilec2cdev.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_SignUpSignIn&client_id=b770840b-2047-46c6-8d55-09aa23cb6bbe&nonce=defaultNonce&redirect_uri=https%3A%2F%2Flocalhost%3A44318%2Ftokenreceiver&scope=openid&response_type=id_token&prompt=login");
            // Perform various tasks depending on the target View.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
    }
}
