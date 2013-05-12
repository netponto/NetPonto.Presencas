using System;
using System.Web.Mvc;
using System.Web.Routing;
using MvcApplication1.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using System.Web.Optimization;

namespace MvcApplication1
{
    public class Application : System.Web.HttpApplication
    {
        public static IDocumentStore DocumentStore;


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DocumentStore = new DocumentStore()
                {
                    ConnectionStringName = "RavenHQ"
                }.Initialize();
            IndexCreation.CreateIndexes(typeof(EventEvaluations_ByEvent).Assembly, DocumentStore);

            BundleTable.EnableOptimizations = true;
            RegisterBundles(BundleTable.Bundles);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHubs();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("api/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" }); //Prevent exceptions for favicon


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );
        }

        private static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js/ko")
                            .Include(
                                "~/Scripts/libs/knockout-2.2.1.js",
                                "~/Scripts/libs/knockout.mapping-latest.js"));



            bundles.Add(new ScriptBundle("~/js/libs")
                            .Include(
                                "~/Scripts/libs/json2.js",
                                "~/Scripts/libs/jquery-1.9.1.js",
                                "~/Scripts/libs/md5.js",
                                "~/Scripts/libs/underscore.js",
                                "~/Scripts/libs/*.jquery.js"));

            bundles.Add(new ScriptBundle("~/js/signalr")
                            .Include("~/Scripts/libs/jquery.signalR-1.0.1.js"));

            bundles.Add(new ScriptBundle("~/js/main")
                            .Include("~/Scripts/*.js", "~/Scripts/Models/*.js"));
            bundles.GetBundleFor("~/js/main").Transforms.Clear();
        }

    }
}