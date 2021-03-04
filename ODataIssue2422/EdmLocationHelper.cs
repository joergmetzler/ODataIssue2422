using System;
using System.Net.Http;
using System.Web.Http.Routing;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData.Edm;

namespace ODataIssue2422
{
    internal class EdmLocationHelper
    {
        private static readonly Lazy<Func<ResourceContext, bool, Uri>> generateODataLinkMethod;

        static EdmLocationHelper()
        {
            generateODataLinkMethod = new Lazy<Func<ResourceContext, bool, Uri>>(() =>
            {
                var resultHelpersType = Type.GetType("Microsoft.AspNet.OData.Results.ResultHelpers, Microsoft.AspNet.OData");
                var generateODataLinkMethod = resultHelpersType.GetMethod(
                    "GenerateODataLink",
                    new[] { typeof(ResourceContext), typeof(bool) }
                );

                return (ResourceContext resourceContext, bool isEntityId) =>
                    (Uri)generateODataLinkMethod.Invoke(null, new object[] { resourceContext, isEntityId });
            });
        }

        // https://github.com/OData/WebApi/blob/7.0.0/src/Microsoft.AspNet.OData/Results/ResultHelpers.cs
        // https://github.com/OData/WebApi/blob/eaeed9a2a031b58b73946a91b1c45b52229cc828/src/Microsoft.AspNet.OData.Shared/Results/ResultHelpers.cs
        // https://github.com/OData/WebApi/blob/eaeed9a2a031b58b73946a91b1c45b52229cc828/src/Microsoft.AspNet.OData.Shared/EdmModelExtensions.cs
        // https://github.com/OData/WebApi/blob/955ee08511485f9b5ca46a4c9d6736a7e0357e85/src/Microsoft.AspNet.OData.Shared/Formatter/ClrTypeCache.cs https://github.com/OData/WebApi/blob/eaeed9a2a031b58b73946a91b1c45b52229cc828/src/Microsoft.AspNet.OData.Shared/Formatter/EdmLibHelpers.cs
        public static Uri GenerateODataLink(HttpRequestMessage request, IEdmEntityObject entity, bool isEntityId)
        {
            var model = request.GetModel();
            var path = request.ODataProperties().Path;
            var navigationSource = path.NavigationSource;

            var resourceContext = new ResourceContext(
                new ODataSerializerContext
                {
                    NavigationSource = navigationSource,
                    Model = model,
                    Url = request.GetUrlHelper() ?? new UrlHelper(request),
                    MetadataLevel = ODataMetadataLevel.FullMetadata, // Used internally to always calculate the links.
                    Request = request,
                    Path = path
                },
                entity.GetEdmType().AsEntity(),
                entity
            );

            return GenerateODataLink(resourceContext, isEntityId);
        }

        private static Uri GenerateODataLink(ResourceContext resourceContext, bool isEntityId)
        {
            return generateODataLinkMethod.Value(resourceContext, isEntityId);
        }
    }
}