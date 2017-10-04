using NJsonSchema;

namespace NitroNet.ModelBuilder
{
    internal class SchemaModel
    {
        public JsonSchema4 Schema { get; set; }
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public bool ShouldGenerateDataAnnotation { get; set; }
    }
}
