using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NitroNet.ViewEngine.Config
{
	public class NitroNetJsonConfig
	{
        public IEnumerable<string> ViewPaths { get; set; }
        public IEnumerable<string> PartialPaths { get; set; }
        public IEnumerable<string> ComponentPaths { get; set; }
        public IEnumerable<string> Extensions { get; set; }
        public IEnumerable<string> Filters { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LiteralParsingMode LiteralParsingMode { get; set; }
        public  bool AdditionalArgumentsOnlyComponents { get; set; }

    }
}