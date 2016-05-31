namespace NitroNet.ViewEngine.IO
{
	public interface IPathHelper
	{
		PathInfo Combine(params PathInfo[] parts);
        PathInfo GetDirectoryName(PathInfo filePath);
        PathInfo ChangeExtension(PathInfo fileName, string extension);
        PathInfo GetFileNameWithoutExtension(PathInfo path);
		string GetExtension(PathInfo path);
	}
}