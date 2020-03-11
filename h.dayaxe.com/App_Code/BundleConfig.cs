using System.Web.Optimization;

public class BundleConfig
{
    // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkID=303951
    public static void RegisterBundles(BundleCollection bundles)
    {
        bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-2.2.4.min.js",
                "~/Scripts/bootstrap.js"));
        bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui.min.js"));
        bundles.Add(new ScriptBundle("~/bundles/bookingdetail").Include(
                "~/Scripts/moment.min.js",
                "~/Scripts/jquery.ignore.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/bootstrap-datetimepicker.min.js",
                "~/Scripts/bookingdetails.js"));

        bundles.Add(new ScriptBundle("~/bundles/calendar").Include(
                "~/Scripts/jquery.ignore.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/dateData.js",
                "~/Scripts/calendar.js"));
    }
}