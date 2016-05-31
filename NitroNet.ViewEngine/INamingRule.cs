using Newtonsoft.Json.Schema;

namespace NitroNet.ViewEngine
{
    public interface INamingRule
    {
        string GetClassName(JSchema schema, string propertyName);
        string GetClassNameFromArrayItem(JSchema schema, string propertyName);
        string GetPropertyName(string input);
        string GetNamespaceName(JSchema schema);
    }
}