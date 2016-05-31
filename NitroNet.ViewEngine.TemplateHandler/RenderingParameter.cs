namespace NitroNet.ViewEngine.TemplateHandler
{
    public class RenderingParameter
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public bool IsDynamic { get; set; }

        public RenderingParameter(string name)
        {
            Name = name;
        }
    }
}
