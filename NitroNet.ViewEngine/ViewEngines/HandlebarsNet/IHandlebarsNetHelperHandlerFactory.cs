using System.Collections.Generic;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public interface IHandlebarsNetHelperHandlerFactory
    {
        IEnumerable<IHandlebarsNetHelperHandler> Create();
    }
}
