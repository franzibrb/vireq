using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using WebAppl.CustomAuthentication;

namespace WebAppl
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            log4net.Config.XmlConfigurator.Configure();

            AuthenticateRequest += Application_AuthenticateRequest;
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if(User != null && User.Identity.IsAuthenticated)
            {
                
            }
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies["AuthCookie"];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                CustomPrincipal principal = new CustomPrincipal(authTicket.Name, int.Parse(authTicket.UserData));
                HttpContext.Current.User = principal;
            }

        }


    }
}
