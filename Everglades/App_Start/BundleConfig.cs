﻿using System.Web;
using System.Web.Optimization;

namespace Everglades
{
    public class BundleConfig
    {
        // Pour plus d'informations sur Bundling, accédez à l'adresse http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/css/wobble.css",
                "~/Content/css/bootstrap.css",
                "~/Content/css/font-awesome.css",
                "~/Content/css/stylish-portfolio.css"));

            bundles.Add(new ScriptBundle("~/Content/js").Include(
                        "~/Content/js/bootstrap.js",
                        "~/Content/js/jquery.js",
                        "~/Content/js/jquery.flot.js",
                        "~/Content/js/jquery.flot.time.js",
                        "~/Content/js/jquery.flot.pie.js",
                        "~/Content/js/jquery.flot.navigate.js",
                        "~/Content/js/stylish-portfolio.js",
                        "~/Content/js/index.js"));

            /*
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
             */
        }
    }
}