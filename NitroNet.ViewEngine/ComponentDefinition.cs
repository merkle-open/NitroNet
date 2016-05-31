using System.Collections.Generic;

namespace NitroNet.ViewEngine
{
    public class ComponentDefinition
    {
        public string Id { get; private set; }

        public ComponentDefinition(string id, FileTemplateInfo defaultTemplate, IReadOnlyDictionary<string, FileTemplateInfo> skins)
        {
            Id = id;
            DefaultTemplate = defaultTemplate;
            Skins = skins;
        }

        public FileTemplateInfo DefaultTemplate { get; private set; }

        public IReadOnlyDictionary<string, FileTemplateInfo> Skins { get; private set; }
    }
}
