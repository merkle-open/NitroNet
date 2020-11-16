using System;

namespace NitroNet.ViewEngine.TemplateHandler
{
    public class RenderingParameter
    {
        public string Name { get; set; }

        [Obsolete("Use method GetValueAsString instead.", true)]
        public string Value { get; set; }

        public object ValueObject { get; set; }

        [Obsolete("Use RenderingParameterType instead.")]
        public bool IsDynamic { get; set; }

        public RenderingParameterType Type { get; set; }

        public RenderingParameter(string name)
        {
            Name = name;
        }

        public string GetValueAsString()
        {
            if (ValueObject == null)
            {
                return null;
            }

            if (Type != RenderingParameterType.Resolved)
            {
                return ValueObject.ToString();
            }

            if (ValueObject is string valueString)
            {
                return valueString;
            }

            return null;
        }
    }

    public enum RenderingParameterType
    {
        StringValue,
        Unresolved,
        Resolved
    }

    public static class ComponentConstants
    {
        public const string ThisIdentifier = "this";
        public const string SkinParameter = "template";
        public const string DataParameter = "data";
        public const string Name = "name";
    }
}
