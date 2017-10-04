using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;

namespace NitroNet.ModelBuilder
{
    public class NitroNetTypeNameGenerator : ITypeNameGenerator
    {
        public string Generate(JsonSchema4 schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
        {
            throw new NotImplementedException();
        }
    }
}
