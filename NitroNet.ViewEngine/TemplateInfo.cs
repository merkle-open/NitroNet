using System.IO;

namespace NitroNet.ViewEngine
{
    public abstract class TemplateInfo
    {
        public string Id { get; private set; }

        protected TemplateInfo(string id)
        {
            Id = id;
        }

        public abstract Stream Open();

        public abstract string ETag { get; }
    }
}
