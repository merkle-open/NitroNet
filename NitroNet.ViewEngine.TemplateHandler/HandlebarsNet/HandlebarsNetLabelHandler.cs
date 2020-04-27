using System;
using HandlebarsDotNet.Compiler;
using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet
{
    public class HandlebarsNetLabelHandler : IHandlebarsNetHelperHandler
    {
        private readonly INitroTemplateHandler _handler;

        public HandlebarsNetLabelHandler(INitroTemplateHandler handler)
        {
            _handler = handler;
        }

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            var parametersAsDictionary = (HashParameterDictionary)parameters.FirstOrDefault();

            if (parametersAsDictionary != null)
            {
                var key = parametersAsDictionary.Keys.First().Trim('"', '\'');

                var viewContext = Sitecore.Mvc.Common.ContextService.Get().GetCurrent<ViewContext>();
                viewContext.Writer = output;

                _handler.RenderLabel(key, viewContext);
            }
        }
    }
}
