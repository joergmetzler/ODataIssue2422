using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData.Edm;
using Microsoft.Owin.Hosting;
using ODataIssue2422.DynamicModel;
using Owin;

namespace ODataIssue2422
{
    internal class Program
    {
        private static void Main()
        {
            var startOptions = new StartOptions("http://localhost:44377");

            using (WebApp.Start(startOptions, Build))
            {
                Console.ReadLine();
            }
        }

        private static void Build(IAppBuilder builder)
        {
            const string RouteName = "odata";
            const string RoutePrefix = "OData";

            var configuration = new HttpConfiguration();

            IEdmModel model = new MyEdmModelBuilder().GetEdmPersonModel();

            var conventions = ODataRoutingConventions.CreateDefaultWithAttributeRouting(RouteName, configuration);
            conventions.Insert(0, new CustomRoutingConvention());

            configuration.MapODataServiceRoute(
                RouteName,
                RoutePrefix,
                model,
                new DefaultODataPathHandler(),
                conventions,
                null
            );

            var controllerSelector = new CustomControllerSelector(configuration);
            configuration.Services.Replace(typeof(IHttpControllerSelector), controllerSelector);

            builder.UseWebApi(configuration);
        }
    }
}