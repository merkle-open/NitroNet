using HandlebarsDotNet;
using System.Collections.Generic;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public interface IHandlebarsNetHelperHandlerFactory
    {
        List<KeyValuePair<string, HandlebarsHelper>> Create();
        List<KeyValuePair<string, HandlebarsBlockHelper>> CreateForBlocks();
    }
}
