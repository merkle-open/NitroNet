using System.IO;
using System.Linq;
using HandlebarsDotNet.Compiler;
using NitroNet.HandlebarsNet.ViewEngine;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.HandlebarsNet.Handlers
{
    public class HandlebarsNetPartialHandler : IHandlebarsNetHelperHandler
    {
        private readonly INitroTemplateHandler _handler;
        private readonly IRenderingContextFactory _renderingContextFactory;

        public HandlebarsNetPartialHandler(INitroTemplateHandler handler, IRenderingContextFactory renderingContextFactory)
        {
            _handler = handler;
            _renderingContextFactory = renderingContextFactory;
        }
        
        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            var parametersAsDictionary = (HashParameterDictionary)parameters.First();

            object value;
            value = parametersAsDictionary.TryGetValue("name", out value) ? value.ToString().Trim('"', '\'') : parametersAsDictionary.First().Key.Trim('"', '\'');

            var template = value.ToString();
            var renderingContext = _renderingContextFactory.GetOrCreate(output, context);

            _handler.RenderPartial(template, context, renderingContext);
        }
    }
}
