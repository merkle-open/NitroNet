using System.Collections.Generic;

namespace NitroNet.ViewEngine.Config
{
	public class NitroNetJsonConfig
	{
        public IEnumerable<string> ViewPaths { get; set; }
        public IEnumerable<string> PartialPaths { get; set; }
        public IEnumerable<string> ComponentPaths { get; set; }
        public IEnumerable<string> Extensions { get; set; }
        public IEnumerable<string> Filters { get; set; }
    }
}