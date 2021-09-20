using System.Web.Optimization;

namespace ArkaApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                 "~/Scripts/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/jquery.sparkline.min.js",
                "~/Scripts/scripts.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/css/app.min.css",
                "~/Content/css/style.css",
                "~/Content/css/components.css",
                "~/Content/css/custom.css"));
        }
    }
}
