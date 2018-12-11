using System.Collections.Generic;
using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet
{
    public class HandlebarsNetHelperHandlerFactory : IHandlebarsNetHelperHandlerFactory
    {
        private readonly INitroTemplateHandlerFactory _nitroTemplateHandlerFactory;

        public HandlebarsNetHelperHandlerFactory(INitroTemplateHandlerFactory nitroTemplateHandlerFactory)
        {
            _nitroTemplateHandlerFactory = nitroTemplateHandlerFactory;
        }

        public IEnumerable<IHandlebarsNetHelperHandler> Create()
        {
            yield return new HandlebarsNetComponentHandler(_nitroTemplateHandlerFactory.Create());
        }
    }
}
