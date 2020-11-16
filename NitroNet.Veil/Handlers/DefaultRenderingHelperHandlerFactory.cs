using System;
using System.Collections.Generic;
using NitroNet.Veil.Handlers.Grid;
using NitroNet.ViewEngine.TemplateHandler;
using Veil.Helper;

namespace NitroNet.Veil.Handlers
{
    [Obsolete("Use HandlebarsNetHelperHandlerFactory instead")]
    public class DefaultRenderingHelperHandlerFactory : IHelperHandlerFactory
    {
        private readonly INitroTemplateHandlerFactory _nitroTemplateHandlerFactory;

	    public DefaultRenderingHelperHandlerFactory(INitroTemplateHandlerFactory nitroTemplateHandlerFactory)
        {
            _nitroTemplateHandlerFactory = nitroTemplateHandlerFactory;
        }

        public IEnumerable<IHelperHandler> Create()
        {
            //Nitro helpers
            yield return new ComponentHelperHandler(_nitroTemplateHandlerFactory.Create());
            yield return new PartialHelperHandler(_nitroTemplateHandlerFactory.Create());
            yield return new PlaceholderHelperHandler(_nitroTemplateHandlerFactory.Create());
            yield return new LabelHelperHandler(_nitroTemplateHandlerFactory.Create());
            yield return new GridHelperHandler();
            yield return new GridWidthHelperHandler();
			yield return new GridComponentWidthHelperHandler();
            yield return new TemplateIdHelperHandler();
        }
    }
}