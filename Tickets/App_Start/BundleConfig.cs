using System.Web;
using System.Web.Optimization;

namespace Tickets
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Bootstrap Styles
            bundles.Add(new StyleBundle("~/bundles/BootstrapStyles").Include(
                      "~/Content/bootstrap.css"));
            // Bootstrap Styles
            bundles.Add(new StyleBundle("~/bundles/ReportStyles").Include(
                      "~/Content/reports.css"));
            // Application Styles
            bundles.Add(new StyleBundle("~/bundles/ApplicationStyles").Include(
                      "~/Content/styles.css",
                      "~/Vendor/datatables/media/css/jquery-dataTables-min.css",
                      "~/Vendor/alertify/themes/alertify-core.css",
                      "~/Vendor/alertify/themes/alertify-bootstrap.css",
                      "~/Vendor/datepicker/css/datepicker.css",
                      "~/Vendor/select2/dist/css/select2-min.css",
                      "~/Vendor/angular-xeditable/dist/css/xeditable.css"
                      ));
            // Application Script
            bundles.Add(new ScriptBundle("~/bundles/ApplicationScripts")
                .Include("~/Scripts/app.module.js")
                .IncludeDirectory("~/Scripts/modules", "*.js", true)
            );

            //----------------------
            //  TICKET PRINT
            //----------------------

            bundles.Add(new ScriptBundle("~/bundles/TicketPrint").Include(
              "~/Vendor/jquery/dist/jquery.js",
              "~/Vendor/JsBarcode/EAN_UPC.js",
              "~/Vendor/JsBarcode/CODE128.js",
              "~/Vendor/JsBarcode/JsBarcode.js",
              "~/Scripts/custom/tickestMain.js"
            ));
            //----------------------
            // Vendor Bundle
            //----------------------

            bundles.Add(new ScriptBundle("~/bundles/VendorScripts").Include(
              "~/Vendor/jquery/dist/jquery.js",
              "~/Vendor/angular/angular.js",
              "~/Vendor/bootstrap-min.js",
              "~/Vendor/angular-animate/angular-animate.js",
              "~/Vendor/angular-bootstrap/ui-bootstrap-tpls.js",
              "~/Vendor/angular-cookies/angular-cookies.js",
              "~/Vendor/angular-dynamic-locale/dist/tmhDynamicLocale.js",
              "~/Vendor/angular-loading-bar/build/loading-bar.js",
              "~/Vendor/angular-resource/angular-resource.js",
              "~/Vendor/angular-route/angular-route.js",
              "~/Vendor/angular-sanitize/angular-sanitize.js",
              "~/Vendor/angular-touch/angular-touch.js",
              "~/Vendor/chart-js/Chart.js",
              "~/Vendor/angular-translate/angular-translate.js",
              "~/Vendor/angular-translate-loader-static-files/angular-translate-loader-static-files.js",
              "~/Vendor/angular-translate-loader-url/angular-translate-loader-url.js",
              "~/Vendor/angular-translate-storage-cookie/angular-translate-storage-cookie.js",
              "~/Vendor/angular-translate-storage-local/angular-translate-storage-local.js",
              "~/Vendor/angular-ui-router/release/angular-ui-router.js",
              "~/Vendor/angular-ui-utils/ui-utils.js",
              "~/Vendor/ng-file-upload/dist/ng-file-upload.js",
              "~/Vendor/ng-file-upload/dist/ng-file-upload-shim.js",
              "~/Vendor/modernizr/modernizr.js",
              "~/Vendor/jquery-formatCurrency/jquery-formatCurrency.js",
              "~/Vendor/jquery-formatCurrency/i18n/jquery-formatCurrency-all.js",
              "~/Vendor/ngstorage/ngStorage.js",
              "~/Vendor/oclazyload/dist/ocLazyLoad.js",
              "~/Vendor/datatables/media/js/jquery-dataTables-min.js",
              "~/Vendor/alertify/lib/alertify-min.js",
              "~/Vendor/datepicker/js/bootstrap-datepicker.js",
              "~/Vendor/select2/dist/js/select2-min.js",
              "~/Vendor/jquery-number.js",
              "~/Vendor/slimscroll/jquery-slimscroll-min.js",
              "~/Vendor/angular-xeditable/dist/js/xeditable-min.js",
              "~/Vendor/loadingBar.js",
              "~/Vendor/JsBarcode/EAN_UPC.js",
              "~/Vendor/JsBarcode/CODE128.js",
              "~/Vendor/JsBarcode/JsBarcode.js",
              "~/Scripts/custom/tickestMain.js"
            ));
        }
    }
}
