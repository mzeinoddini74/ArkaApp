using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ArkaApp.Helper
{
    public class UserAccessAttribute : ActionFilterAttribute
    {
        public string ControllerName { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket != null)
                {
                    var userData = authTicket.UserData.Split(',').ToArray();
                    if (userData.Length != 3)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Account" }));
                    }
                    else
                    {
                        if (!Utility.HasUserAccess(HttpContext.Current.User.Identity.Name, ControllerName))
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Dashboard" }));
                        }
                    }
                }
            }
            else
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Home" }));
        }
    }

}