using System.Linq;
using System.Web.Http.Controllers;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using KeySegment = Microsoft.OData.UriParser.KeySegment;

namespace ODataIssue2422.DynamicModel
{
    internal class CustomRoutingConvention : NavigationSourceRoutingConvention
    {
        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (controllerContext.Controller is MetadataController)
            {
                return null;
            }

            if (odataPath.PathTemplate == "~/entityset/key")
            {
                if (odataPath.Segments.Count > 1 && odataPath.Segments[1] is KeySegment keySegment)
                {
                    controllerContext.RouteData.Values[ODataRouteConstants.Key] = keySegment.Keys.First().Value;
                }

                return "Get";
            }

            return null;
        }
    }
}