using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NitroNet.ViewEngine.IO
{
	public class EmbeddedResourceFileSystem : IFileSystem
	{
		private static readonly IPathHelper PathHelper = new FileSystem.FilePathHelper();
		private readonly Assembly _assembly;
		private readonly IDictionary<PathInfo, string> _names;
		private readonly string _etag;

		public EmbeddedResourceFileSystem(Assembly assembly)
		{
			_assembly = assembly;
			_names = _assembly.GetManifestResourceNames().ToDictionary(s => Path.Combine(PathInfo.Create(s)), s => s);
			_etag = _assembly.GetName().Version.ToString(4);
		}

		public PathInfo BasePath
		{
			get { return PathInfo.Empty; }
		}

		public bool DirectoryExists(PathInfo directory)
		{
			return _names.ContainsKey(directory);
		}

		public IEnumerable<PathInfo> DirectoryGetFiles(PathInfo directory, string fileExtension)
		{
			throw new NotSupportedException();
		}

		public Stream OpenRead(PathInfo filePath)
		{
			string resourceName;
			if (!_names.TryGetValue(filePath, out resourceName))
				throw new ArgumentException("Invalid file path");

			return _assembly.GetManifestResourceStream(resourceName);
		}

		public Stream OpenWrite(PathInfo filePath)
		{
			throw new NotSupportedException();
		}

		public bool FileExists(PathInfo filePath)
		{
			return _names.ContainsKey(filePath);
		}

		public void RemoveFile(PathInfo filePath)
		{
			throw new NotSupportedException();
		}

		public void CreateDirectory(PathInfo directory)
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
			throw new NotImplementedException();
		}

		public IFileInfo GetFileInfo(PathInfo filePath)
		{
			return new EmbeddedFileInfo(filePath, _etag);
		}

		private class EmbeddedFileInfo : IFileInfo
		{
			public PathInfo FilePath { get; private set; }
			public string Etag { get; private set; }

			public EmbeddedFileInfo(PathInfo filePath, string etag)
			{
				FilePath = filePath;
				Etag = etag;
			}
		}
	}
}