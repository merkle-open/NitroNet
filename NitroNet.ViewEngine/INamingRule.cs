using Newtonsoft.Json.Schema;
using System;

namespace NitroNet.ViewEngine
{
    [Obsolete("will be removed as soon final switch to handlebarsnet is completed")]
    public interface INamingRule
    {
        string GetClassName(JSchema schema, string propertyName);
        string GetClassNameFromArrayItem(JSchema schema, string propertyName);
        string GetPropertyName(string input);
        string GetNamespaceName(JSchema schema);
    }
}