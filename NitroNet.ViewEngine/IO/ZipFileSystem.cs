using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace NitroNet.ViewEngine.IO
{
    public class ZipFileSystem : IFileSystem
    {
        private readonly PathInfo _filePath;
        private readonly PathInfo _rootPath;
        private readonly string _rootPathString;
        private readonly int _rootPathLength;
        private static readonly IPathHelper PathHelper = new ZipPathHelper();
        private readonly ZipFile _file;
        private readonly string _etag;

        public ZipFileSystem(string filePath, string rootPath)
        {
            _filePath = PathInfo.Create(filePath);
            _rootPath = PathInfo.Create(rootPath);
            _rootPathString = _rootPath.ToString();
            _rootPathLength = _rootPathString.Length;
            _file = new ZipFile(filePath);
            _etag = new FileInfo(filePath).LastWriteTimeUtc.Ticks.ToString("X8");
        }

        public PathInfo BasePath { get { return _filePath; } }

        public bool DirectoryExists(PathInfo directory)
        {
            var entryName = GetFullPath(directory);
            return _file.OfType<ZipEntry>().Any(e => e.Name.StartsWith(entryName));
        }

        public IEnumerable<PathInfo> DirectoryGetFiles(PathInfo directory, string fileExtension)
        {
            return _file.OfType<ZipEntry>()
                .Where(e => e.IsFile && e.Name.StartsWith(GetFullPath(directory)) && e.Name.EndsWith(string.Concat(".", fileExtension)))
                .Select(e => PathInfo.Create(e.Name.Substring(_rootPathLength + 1)));
        }

		public Stream OpenRead(PathInfo filePath)
        {
            var file = _file.GetEntry(GetFullPath(filePath).ToString());
            return _file.GetInputStream(file);
        }

		public Stream OpenWrite(PathInfo filePath)
        {
            throw new NotSupportedException();
        }

        public bool FileExists(PathInfo filePath)
        {
            var dir = _file.GetEntry(GetFullPath(filePath).ToString());
            return dir != null;
        }

        public void RemoveFile(PathInfo filePath)
	    {
	        throw new NotSupportedException();
	    }

        public Stream OpenReadOrCreate(PathInfo filePath)
	    {
            throw new NotSupportedException();
	    }

        public IPathHelper Path
        {
            get { return PathHelper; }
        }

	    public bool SupportsSubscribe
	    {
		    get { return false; }
	    }

	    public Task<IDisposable> SubscribeAsync(string pattern, Action<IFileInfo> handler)
	    {
			throw new NotSupportedException();
	    }

		public Task<IDisposable> SubscribeDirectoryGetFilesAsync(PathInfo prefix, string extension, Action<IEnumerable<IFileInfo>> handler)
		{
			throw new NotSupportedException();
	    }

	    public IFileInfo GetFileInfo(PathInfo filePath)
	    {
		    return new ZipFileInfo(filePath, _etag);
	    }

	    public void CreateDirectory(PathInfo directory)
        {
            throw new NotSupportedException();
        }

        private string GetFullPath(PathInfo path)
        {
            if (path == null)
                return _rootPathString;

            return Path.Combine(_rootPath, path).ToString();
        }

		private class ZipFileInfo : IFileInfo
		{
			public PathInfo FilePath { get; private set; }

			public string Etag { get; private set; }

			public ZipFileInfo(PathInfo filePath, string etag)
			{
				FilePath = filePath;
				Etag = etag;
			}
		}

        private class ZipPathHelper : IPathHelper
        {
            public PathInfo Combine(params PathInfo[] parts)
            {
                return PathInfo.Combine(parts);
                //return PathInfo.Create(PathUtility.Combine(parts.Select(s => s.ToString()).ToArray()));
            }

            public PathInfo GetDirectoryName(PathInfo filePath)
            {
                return PathInfo.Create(System.IO.Path.GetDirectoryName(filePath.ToString()));
            }

            public PathInfo ChangeExtension(PathInfo fileName, string extension)
            {
                return PathInfo.Create(System.IO.Path.ChangeExtension(fileName.ToString(), extension));
            }

            public PathInfo GetFileNameWithoutExtension(PathInfo path)
            {
                return PathInfo.Create(System.IO.Path.GetFileNameWithoutExtension(path.ToString()));
            }

            public string GetExtension(PathInfo path)
            {
                return System.IO.Path.GetExtension(path.ToString());
            }
        }
    }
}