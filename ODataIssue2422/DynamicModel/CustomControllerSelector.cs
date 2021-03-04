using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Microsoft.AspNet.OData;
using ODataIssue2422.Controllers;

namespace ODataIssue2422.DynamicModel
{
    /// <summary>
    /// Implementation of the <see cref="CustomControllerSelector"/> class.
    /// The Controller is selected based on the OData path segments.
    /// No odata path segments or $metadata selects <see cref="MetadataController"/>.
    /// Everything else selects <see cref="CluuODataController"/>.
    /// </summary>
    internal class CustomControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomControllerSelector"/> class.
        /// </summary>
        public CustomControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Selects the Controller that provides the requested data.
        /// </summary>
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            if (request.RequestUri.ToString().EndsWith("$metadata"))
            {
                return new HttpControllerDescriptor(this.configuration, "Metadata", typeof(MetadataController));
            }

            return new HttpControllerDescriptor(this.configuration, "MyOData", typeof(MyODataController));
        }
    }
}