using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NitroNet.ViewEngine.IO
{
	public interface IFileSystem
	{
		PathInfo BasePath { get; }

		bool DirectoryExists(PathInfo directory);
		IEnumerable<PathInfo> DirectoryGetFiles(PathInfo directory, string fileExtension);
		Stream OpenRead(PathInfo filePath);
		Stream OpenWrite(PathInfo filePath);
		bool FileExists(PathInfo filePath);
		void RemoveFile(PathInfo filePath);
		void CreateDirectory(PathInfo directory);
		Stream OpenReadOrCreate(PathInfo filePath);
		IPathHelper Path { get; }
		bool SupportsSubscribe { get; }
		Task<IDisposable> SubscribeAsync(string pattern, Action<IFileInfo> handler);
		Task<IDisposable> SubscribeDirectoryGetFilesAsync(PathInfo prefix, string extension,
			Action<IEnumerable<IFileInfo>> handler);
		IFileInfo GetFileInfo(PathInfo filePath);
	}

	public interface IFileInfo
	{
		PathInfo FilePath { get; }
		string Etag { get; }
	}
}