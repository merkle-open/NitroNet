using System.Collections.Generic;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine.Config
{
	public interface INitroNetConfig
    {
        IEnumerable<PathInfo> ViewPaths { get; }
        IEnumerable<PathInfo> PartialPaths { get; }
        IEnumerable<PathInfo> ComponentPaths { get; }
        IEnumerable<string> Extensions { get; }
	}
}
