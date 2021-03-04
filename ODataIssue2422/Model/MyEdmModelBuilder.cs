using Microsoft.OData.Edm;

namespace ODataIssue2422
{
    internal class MyEdmModelBuilder
    {
        public EdmModel GetEdmPersonModel()
        {
            string ns = "MyNamespace";

            var person = new EdmEntityType(ns, "Person");
            var id = person.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            person.AddKeys(id);
            person.AddStructuralProperty("FirstName", EdmPrimitiveTypeKind.String);
            person.AddStructuralProperty("LastName", EdmPrimitiveTypeKind.String);

            var container = new EdmEntityContainer(ns, ns + "Container");
            container.AddEntitySet("Persons", person);

            var model = new EdmModel();
            model.AddElement(person);
            model.AddElement(container);

            return model;
        }
    }
}