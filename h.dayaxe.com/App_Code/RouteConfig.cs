using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

public static class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.Ignore("favicon.ico");
        routes.MapPageRoute("Default", "", "~/Default.aspx");
        var settings = new FriendlyUrlSettings();
        settings.AutoRedirectMode = RedirectMode.Off;

        routes.EnableFriendlyUrls(settings);
    }
}