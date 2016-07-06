using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NitroNet.ViewEngine.IO
{
    public class FileSystemProvider
    {
        public IFileSystem GetFileSystem(string hostPath, string path, out string basePath)
        {
            IFileSystem fileSystem = null;
            if (path.StartsWith("zip://"))
            {
                string baseInFile;
                var filePath = GetFilePath(new Uri(path, UriKind.RelativeOrAbsolute), out baseInFile);
                filePath = Path.GetFullPath(!Path.IsPathRooted(filePath) ? Path.Combine(hostPath, filePath) : filePath);

                fileSystem = CreateZipFileSystem(filePath, baseInFile);
                basePath = string.Empty;
            }
            else
            {
				basePath = PathInfo.Combine(PathInfo.Create(hostPath), PathInfo.Create(path)).ToString();
                basePath = string.Empty;
            }
            return fileSystem;
        }

        protected internal virtual ZipFileSystem CreateZipFileSystem(string filePath, string baseInFile)
        {
            return new ZipFileSystem(filePath, baseInFile);
        }

        private static string GetFilePath(Uri uri, out string rootPath)
        {
            var pathBuilder = new StringBuilder();
            var result = string.Empty;

            var host = uri.IsAbsoluteUri ? new[] {uri.Host} : new string[0];

            foreach (var segment in host.Union(uri.Segments).Where(s => !string.IsNullOrEmpty(s) && s != "/"))
            {
                var part = segment.TrimEnd('/');
                pathBuilder.Append(part);

                if (!string.IsNullOrEmpty(Path.GetExtension(part)))
                {
                    result = pathBuilder.ToString();
                    pathBuilder.Clear();
                }
                else
                {
                    pathBuilder.Append('/');
                }
            }

            rootPath = pathBuilder.ToString();

            return result;
        }
    }
}