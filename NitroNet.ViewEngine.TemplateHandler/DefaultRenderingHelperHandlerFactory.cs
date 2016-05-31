using System.Collections.Generic;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using Veil.Helper;

namespace NitroNet.ViewEngine.TemplateHandler
{
    public class DefaultRenderingHelperHandlerFactory : IHelperHandlerFactory
    {
        private readonly INitroTemplateHandlerFactory _nitroTemplateHandlerFactory;

	    public DefaultRenderingHelperHandlerFactory(INitroTemplateHandlerFactory nitroTemplateHandlerFactory)
        {
            _nitroTemplateHandlerFactory = nitroTemplateHandlerFactory;
        }

        public IEnumerable<IHelperHandler> Create()
        {
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