using System.Collections.Generic;
using System.IO;
using System.Linq;
using HandlebarsDotNet.Compiler;
using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet
{
    public class HandlebarsNetComponentHandler : IHandlebarsNetHelperHandler
    {
        private readonly INitroTemplateHandler _handler;

        public HandlebarsNetComponentHandler(INitroTemplateHandler handler)
        {
            _handler = handler;
        }

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            RenderingParameter template;
            var parametersAsDictionary = GetParametersAsDictionary(parameters);

            var firstParameter = parametersAsDictionary.FirstOrDefault();
            if (string.IsNullOrEmpty(firstParameter.Value.ToString()))
            {
                template = new RenderingParameter("name")
                {
                    Value = firstParameter.Key.Trim('"', '\'')
                };
            }
            else
            {
                template = CreateRenderingParameter("name", parametersAsDictionary);
            }

            var skin = CreateRenderingParameter("template", parametersAsDictionary);
            var dataVariation = CreateRenderingParameter("data", parametersAsDictionary);

            _handler.RenderComponent(template, skin, dataVariation, context, new RenderingContext(output));
        }

        private RenderingParameter CreateRenderingParameter(string name, IDictionary<string, object> parameters)
        {
            var renderingParameter = new RenderingParameter(name);

            if (parameters.ContainsKey(renderingParameter.Name))
            {
                var value = parameters[renderingParameter.Name];

                if (!value.ToString().StartsWith("\"") && !value.ToString().StartsWith("'"))
                {
                    renderingParameter.IsDynamic = true;
                }

                renderingParameter.Value = value.ToString().Trim('"', '\'');
            }

            return renderingParameter;
        }

        private IDictionary<string, object> GetParametersAsDictionary(params object[] parameters)
        {
            return (HashParameterDictionary)parameters.First();
        }
    }
}
