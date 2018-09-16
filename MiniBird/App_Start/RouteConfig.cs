using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MiniBird
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ProfileIdTabs",
                url: "Account/ProfileScreen/{id}/{v}",
                defaults: new { controller = "Account", action = "ProfileScreen", v = UrlParameter.Optional }
                //constraints: new { id = new UserNameConstraint() }
            );            

            routes.MapRoute(
                name: "ViewPost",
                url: "Account/ProfileScreen/ViewPost/{postID}",
                defaults: new { controller = "Account", action = "ViewPost" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Welcome", id = UrlParameter.Optional }
            );
        }        

        //public class UserNameConstraint : IRouteConstraint
        //{
        //    public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        //    {
        //        List<string> users = new List<string>() { "Bryan", "Stephen" };
        //        //Get the username from the url
        //        var username = values["username"].ToString().ToLower();
        //        //Check for a match (assumes case insensitive)
        //        return users.Any(x => x.ToLower() == username);
        //    }
        //}
    }
}
