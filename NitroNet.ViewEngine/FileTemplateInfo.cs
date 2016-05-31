using System.IO;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine
{
    public class FileTemplateInfo : TemplateInfo
    {
        private readonly TemplateType _type;
        private readonly PathInfo _filePath;
        private readonly IFileSystem _fileSystem;

        public FileTemplateInfo(string id, TemplateType type, PathInfo filePath, IFileSystem fileSystem)
            : base(id)
        {
            _type = type;
            _filePath = filePath;
            _fileSystem = fileSystem;
        }

        public override Stream Open()
        {
            return _fileSystem.OpenRead(_filePath);
        }

        public TemplateType Type
        {
            get { return _type; }
        }

        public PathInfo Path
        {
            get { return _filePath; }
        }

        public string Name
        {
            get { return _fileSystem.Path.GetFileNameWithoutExtension(_filePath).RemoveStartSlash().ToString(); }
        }

        public override string ETag
        {
            get { return _fileSystem.GetFileInfo(_filePath).Etag; }
        }
    }

    public enum TemplateType
    {
        View,
        Partial,
        Component
    }
}
