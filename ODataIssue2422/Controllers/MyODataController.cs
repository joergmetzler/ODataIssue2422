using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;

namespace ODataIssue2422.Controllers
{
    public class MyODataController : ODataController
    {
        // wget -UseBasicParsing 'http://localhost:44377/OData/Persons'
        public IHttpActionResult Get()
        {
            var path = this.Request.ODataProperties().Path;
            var edmCollectionType = (IEdmCollectionType)path.EdmType;
            var edmEntityType = (IEdmEntityType)edmCollectionType.ElementType.Definition;

            var result = new EdmEntityObjectCollection(
                new EdmCollectionTypeReference(edmCollectionType),
                new List<IEdmEntityObject> { CreatePerson(edmEntityType, 1), CreatePerson(edmEntityType, 2) }
            );

            return this.Ok(result);
        }

        // wget -UseBasicParsing 'http://localhost:44377/OData/Persons(1)'
        public IHttpActionResult Get(string key)
        {
            var path = this.Request.ODataProperties().Path;
            var edmEntityType = (IEdmEntityType)path.EdmType;

            var edmEntity = CreatePerson(edmEntityType, int.Parse(key));

            return this.Ok(edmEntity);
        }

        // wget -UseBasicParsing 'http://localhost:44377/OData/Persons' -Method POST -Body '{"@odata.context":"http://localhost:44377/OData/$metadata#Persons/$entity","FirstName":"J\u00f6rg","LastName":"Metzler"}'
        public IHttpActionResult Post()
        {
            var path = this.Request.ODataProperties().Path;
            var edmCollectionType = (IEdmCollectionType)path.EdmType;
            var edmEntityType = (IEdmEntityType)edmCollectionType.ElementType.Definition;

            // TODO: Create person from request data
            var edmEntity = CreatePerson(edmEntityType, 1);

            // TODO: Save...

            // Bug
            // return this.Created(edmEntity);

            // Works
            return this.Created(
                EdmLocationHelper.GenerateODataLink(this.Request, edmEntity, isEntityId: false),
                edmEntity
            );
        }

        private static IEdmEntityObject CreatePerson(IEdmEntityType edmType, int id)
        {
            var edmEntity = new EdmEntityObject(edmType);
            edmEntity.TrySetPropertyValue("Id", id);
            edmEntity.TrySetPropertyValue("FirstName", "Jörg " + id);
            edmEntity.TrySetPropertyValue("LastName", "Metzler " + id);
            return edmEntity;
        }
    }
}