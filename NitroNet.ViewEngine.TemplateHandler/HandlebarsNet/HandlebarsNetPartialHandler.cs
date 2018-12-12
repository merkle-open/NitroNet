using HandlebarsDotNet.Compiler;
using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet
{
    public class HandlebarsNetPartialHandler : IHandlebarsNetHelperHandler
    {
        private readonly INitroTemplateHandler _handler;

        public HandlebarsNetPartialHandler(INitroTemplateHandler handler)
        {
            _handler = handler;
        }

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            var parametersAsDictionary = (HashParameterDictionary)parameters.First();

            object value;
            value = parametersAsDictionary.TryGetValue("name", out value) ? value.ToString().Trim('"', '\'') : parametersAsDictionary.First().Key.Trim('"', '\'');

            var template = value.ToString();
            var viewContext = Sitecore.Mvc.Common.ContextService.Get().GetInstances<ViewContext>();
            _handler.RenderPartial(template, context, viewContext.First());
        }
    }
}
