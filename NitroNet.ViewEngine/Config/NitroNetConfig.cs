using System.Collections.Generic;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine.Config
{
    public class NitroNetConfig : INitroNetConfig
    {
        public IEnumerable<PathInfo> ViewPaths { get; set; }
        public IEnumerable<PathInfo> PartialPaths { get; set; }
        public IEnumerable<PathInfo> ComponentPaths { get; set; }
        public IEnumerable<string> Extensions { get; set; }
        public IEnumerable<string> Filters { get; set; }
    }
}
