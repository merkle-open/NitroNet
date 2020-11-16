using System.Collections.Generic;
using HandlebarsDotNet;

namespace NitroNet.HandlebarsNet.ViewEngine
{
    public interface IHandlebarsNetHelperHandlerFactory
    {
        IDictionary<string, HandlebarsHelper> Create();
        IDictionary<string, HandlebarsBlockHelper> CreateForBlocks();
    }
}
