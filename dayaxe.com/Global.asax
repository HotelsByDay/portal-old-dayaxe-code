<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="AutoMapper" %>
<%@ Import Namespace="dayaxe.com" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Import Namespace="Stripe" %>

<script Language="C#" RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        StripeConfiguration.SetApiKey(AppConfiguration.StripeApiKey);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
        BundleTable.EnableOptimizations = true;

        // config Auto Mapper
        Mapper.Initialize(cfg =>
        {
            cfg.CreateMap<Bookings, BookingsTemps>();
            cfg.CreateMap<BookingsTemps, Bookings>();
        });
    }

    protected void Application_AcquireRequestState(object sender, EventArgs e)
    {

    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        try
        {
            Exception exception = Server.GetLastError();
            HttpException httpException = (HttpException)exception;
            int httpCode = httpException.GetHttpCode();
            CacheLayer.ClearAll();

            switch (httpCode)
            {
                //    case 400:
                //        Response.Redirect("/ErrorPages/conf-error.aspx");
                //        break;
                //    case 401:
                //    case 403:
                //        Response.Redirect("/ErrorPages/unauthorized.aspx");
                //        break;
                case 404:
                    Response.Redirect("/ErrorPages/resource-not-found.aspx");
                    break;
                    //    case 500:
                    //        Response.Redirect("/ErrorPages/conf-error.aspx");
                    //        break;
                    //    default:
                    //        Response.Redirect("/ErrorPages/conf-error.aspx");
                    //        break;
            }
        }
        catch { }
    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
        Session["CustomSessionId"] = Guid.NewGuid();
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
</script>
