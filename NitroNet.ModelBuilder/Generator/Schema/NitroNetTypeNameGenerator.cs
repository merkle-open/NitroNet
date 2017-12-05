using System.Collections.Generic;
using NJsonSchema;

namespace NitroNet.ModelBuilder.Generator.Schema
{
    public class NitroNetTypeNameGenerator : DefaultTypeNameGenerator
    {

        /// <summary>Generates the type name for the given schema respecting the reserved type names.</summary>
        /// <param name="schema">The schema.</param>
        /// <param name="typeNameHint">The type name hint.</param>
        /// <param name="reservedTypeNames">The reserved type names.</param>
        /// <returns>The type name.</returns>
        public override string Generate(JsonSchema4 schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
        {
            var name = base.Generate(schema, typeNameHint, reservedTypeNames);
            if(!name.StartsWith("Anonymous"))
            {
                return name;
            }
            
            if(schema.Type == JsonObjectType.Array)
            {
                return name.Replace("Anonymous", "ObjectModel");
            }

            if (schema.Type == JsonObjectType.Object)
            {
                return name.Replace("Anonymous", "ItemList");
            }

            return name;
        }
        
    }
}
