using System.Web.Optimization;

namespace dayaxe.com
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/jcookie.js"));
            bundles.Add(new ScriptBundle("~/bundles/shortern").Include(
                "~/assets/js/jquery.shorten.js"));
            bundles.Add(new ScriptBundle("~/bundles/search").Include(
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/jquery.ignore.js",
                "~/assets/js/moment.min.js",
                "~/assets/js/daterangepicker.min.js",
                "~/assets/js/bootstrap-slider.min.js",
                "~/assets/js/skel.min.js",
                "~/assets/js/cloudinary/cloudinary-core.min.js",
                "~/assets/js/util.js",
                "~/assets/js/main.js",
                "~/assets/js/search.js",
                "~/assets/js/newsletter.js"));
            bundles.Add(new ScriptBundle("~/bundles/product").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/bootstrap-datepicker.js",
                "~/assets/js/jquery.ignore.js",
                "~/assets/js/skel.min.js",
                "~/assets/js/util.js",
                "~/assets/js/jquery.lazyload.js",
                "~/assets/js/owl.carousel.min.js",
                "~/assets/js/star-rating.min.js",
                "~/assets/js/jquery.shorten.js",
                "~/assets/js/bootstrap-slider.min.js",
                "~/assets/js/jcookie.js",
                "~/assets/js/toggles/toggles.min.js",
                "~/assets/js/cloudinary/cloudinary-core.min.js",
                "~/assets/js/main.js",
                "~/assets/js/product.js",
                "~/assets/js/newsletter.js"));
            bundles.Add(new ScriptBundle("~/bundles/reviews").Include(
                "~/assets/js/jquery.shorten.js",
                "~/assets/js/reviews.js"));

            bundles.Add(new ScriptBundle("~/bundles/membership").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/membership.js"));
            bundles.Add(new ScriptBundle("~/bundles/book").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/jquery.ignore.js",
                "~/assets/js/bootstrap-datepicker.js",
                "~/assets/js/skel.min.js",
                "~/assets/js/util.js",
                "~/assets/js/owl.carousel.min.js",
                "~/assets/js/jcookie.js",
                "~/assets/jquerycountdown/jquery.plugin.js",
                "~/assets/jquerycountdown/jquery.countdown.js",
                "~/assets/js/cloudinary/cloudinary-core.min.js",
                "~/assets/js/creditCardValidator.js",
                "~/assets/js/main.js",
                "~/assets/js/custom.js",
                "~/assets/js/newsletter.js"));
            bundles.Add(new ScriptBundle("~/bundles/confirm").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/skel.min.js",
                "~/assets/js/util.js",
                "~/assets/js/owl.carousel.min.js",
                "~/assets/js/jcookie.js",
                "~/assets/js/main.js",
                "~/assets/js/confirm.js"));
            bundles.Add(new ScriptBundle("~/bundles/daypass").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/jcookie.js",
                "~/assets/js/jquery.ignore.js",
                "~/assets/js/bootstrap-datepicker.js",
                "~/assets/js/skel.min.js",
                "~/assets/js/atc.min.js",
                "~/assets/js/cloudinary/cloudinary-core.min.js",
                "~/assets/js/viewdaypass.js",
                "~/assets/js/newsletter.js"));
            bundles.Add(new ScriptBundle("~/bundles/default").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/jquery.lazyload.js",
                "~/assets/js/owl.carousel.min.js",
                "~/assets/js/cloudinary/cloudinary-core.min.js",
                "~/assets/js/default.js",
                "~/assets/js/newsletter.js"));


            bundles.Add(new ScriptBundle("~/bundles/booksubscription").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/jquery.ignore.js",
                "~/assets/js/bootstrap-datepicker.js",
                "~/assets/js/skel.min.js",
                "~/assets/js/util.js",
                "~/assets/js/owl.carousel.min.js",
                "~/assets/jquerycountdown/jquery.plugin.js",
                "~/assets/jquerycountdown/jquery.countdown.js",
                "~/assets/js/creditCardValidator.js",
                "~/assets/js/main.js",
                "~/assets/js/bookSubscription.js"));

            bundles.Add(new ScriptBundle("~/bundles/bookgiftcard").Include(
                "~/assets/js/jquery.min.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/jquery.ignore.js",
                "~/assets/js/bootstrap-datepicker.js",
                "~/assets/js/skel.min.js",
                "~/assets/js/util.js",
                "~/assets/js/owl.carousel.min.js",
                "~/assets/js/jcookie.js",
                "~/assets/jquerycountdown/jquery.plugin.js",
                "~/assets/jquerycountdown/jquery.countdown.js",
                "~/assets/js/creditCardValidator.js",
                "~/assets/js/main.js",
                "~/assets/js/bookgiftcard.js"));
        }
    }
}