using System.IO;
using System.Linq;
using HandlebarsDotNet.Compiler;
using NitroNet.HandlebarsNet.ViewEngine;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.HandlebarsNet.Handlers
{
    public class HandlebarsNetLabelHandler : IHandlebarsNetHelperHandler
    {
        private readonly INitroTemplateHandler _handler;
        private readonly IRenderingContextFactory _renderingContextFactory;

        public HandlebarsNetLabelHandler(INitroTemplateHandler handler, IRenderingContextFactory renderingContextFactory)
        {
            _handler = handler;
            _renderingContextFactory = renderingContextFactory;
        }

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            var parametersAsDictionary = (HashParameterDictionary)parameters.FirstOrDefault();

            if (parametersAsDictionary != null)
            {
                var key = parametersAsDictionary.Keys.First().Trim('"', '\'');
                var renderingContext = _renderingContextFactory.GetOrCreate(output, context);

                _handler.RenderLabel(key, renderingContext);
            }
        }
    }
}
