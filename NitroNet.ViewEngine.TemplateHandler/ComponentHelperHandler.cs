using System;
using System.Collections.Generic;
using System.Linq;
using Veil;
using Veil.Helper;

namespace NitroNet.ViewEngine.TemplateHandler
{
    internal class ComponentHelperHandler : IHelperHandler
    {
        private readonly INitroTemplateHandler _handler;

        public ComponentHelperHandler(INitroTemplateHandler handler)
        {
            _handler = handler;
        }

        public bool IsSupported(string name)
        {
            return name.StartsWith("component", StringComparison.OrdinalIgnoreCase);
        }

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
		    RenderingParameter template;
		    var firstParameter = parameters.FirstOrDefault();
		    if (string.IsNullOrEmpty(firstParameter.Value))
		    {
		        template = new RenderingParameter("name")
		        {
		            Value = firstParameter.Key.Trim('"', '\'')
                };
		    }
		    else
		    {
                template = CreateRenderingParameter("name", parameters);
            }

            var skin = CreateRenderingParameter("template", parameters);
		    var dataVariation = CreateRenderingParameter("data", parameters);

            _handler.RenderComponent(template, skin, dataVariation, model, context);
		}

        private RenderingParameter CreateRenderingParameter(string name, IDictionary<string, string> parameters)
        {
            var renderingParameter = new RenderingParameter(name);
            if (parameters.ContainsKey(renderingParameter.Name))
            {
                var value = parameters[renderingParameter.Name];

                if (!value.StartsWith("\"") && !value.StartsWith("'"))
                {
                    renderingParameter.IsDynamic = true;
                }

                renderingParameter.Value = value.Trim('"', '\'');
            }

            return renderingParameter;
        }
    }
}