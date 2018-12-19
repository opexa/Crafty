using System;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Crafty.App
{
  public class BundleConfig
  {
    // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Scripts/jquery-{version}.js"));

      bundles.Add(new ScriptBundle("~/Content/jquery").Include(
                  "~/Scripts/jquery-1.10.2.js",
                  "~/Scripts/jquery.validate.js",
                  "~/Scripts/jquery.unobtrusive-ajax.js",
                  "~/Scripts/jquery.validate.unobtrusive.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.validate*"));

      // Use the development version of Modernizr to develop with and learn from. Then, when you're
      // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                  "~/Scripts/modernizr-*"));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));


      bundles.Add(new StyleBundle("~/HomeIndex/css").Include(
                "~/Stylesheets/index.css").WithLastModifiedToken());

      bundles.Add(new StyleBundle("~/Default/css").Include(
                "~/Stylesheets/bootstrap.simplex.min.css",
                "~/Stylesheets/font-awesome.min.css").WithLastModifiedToken());

      bundles.Add(new StyleBundle("~/ProfileIndex/css").Include(
                "~/Stylesheets/ProfileIndex.css").WithLastModifiedToken());
    }
  }


  internal static class BundleExtensions
  {
    public static Bundle WithLastModifiedToken(this Bundle sb)
    {
      sb.Transforms.Add(new LastModifiedBundleTransform());
      return sb;
    }
    public class LastModifiedBundleTransform : IBundleTransform
    {
      public void Process(BundleContext context, BundleResponse response)
      {
        foreach (var file in response.Files)
        {
          var lastWrite = File.GetLastWriteTime(HostingEnvironment.MapPath(file.IncludedVirtualPath)).Ticks.ToString();
          file.IncludedVirtualPath = string.Concat(file.IncludedVirtualPath, "?v=", lastWrite);
        }
      }
    }
  }
}
