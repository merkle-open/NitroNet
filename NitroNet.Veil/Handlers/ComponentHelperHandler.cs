using System;
using System.Collections.Generic;
using System.Linq;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler;
using Veil.Helper;

namespace NitroNet.Veil.Handlers
{
    public class ComponentHelperHandler : IHelperHandler
    {
        
        private readonly INitroTemplateHandler _handler;

        public ComponentHelperHandler(INitroTemplateHandler handler)
        {
            _handler = handler;
        }

        public bool IsSupported(string name)
        {
            return name.StartsWith("component", StringComparison.OrdinalIgnoreCase) || name.StartsWith("pattern", StringComparison.OrdinalIgnoreCase);
        }

        public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
        {
            var renderingParameters = GetRenderingParameters(parameters);
            _handler.RenderComponent(renderingParameters, model, context, parameters);
        }

        private IDictionary<string, RenderingParameter> GetRenderingParameters(IDictionary<string, string> parameters)
        {
            RenderingParameter template;
            var firstParameter = parameters.FirstOrDefault();
            if (string.IsNullOrEmpty(firstParameter.Value))
            {
                template = new RenderingParameter(ComponentConstants.Name)
                {
                    ValueObject = firstParameter.Key.Trim('"', '\'')
                };
            }
            else
            {
                template = CreateRenderingParameter(ComponentConstants.Name, parameters);
            }

            var renderingParametersDictionary = new Dictionary<string, RenderingParameter>();

            renderingParametersDictionary.Add(ComponentConstants.Name, template);
            AddAdditionalRenderingParameters(parameters, renderingParametersDictionary);

            return renderingParametersDictionary;

        }

        protected virtual void AddAdditionalRenderingParameters(IDictionary<string, string> parameters, Dictionary<string, RenderingParameter> renderingParametersDictionary)
        {
            renderingParametersDictionary.Add(ComponentConstants.SkinParameter,
                CreateRenderingParameter(ComponentConstants.SkinParameter, parameters));
            renderingParametersDictionary.Add(ComponentConstants.DataParameter,
                CreateRenderingParameter(ComponentConstants.DataParameter, parameters));
        }


        protected RenderingParameter CreateRenderingParameter(string name, IDictionary<string, string> parameters)
        {
            var renderingParameter = new RenderingParameter(name);
            if (parameters.ContainsKey(renderingParameter.Name))
            {
                var value = parameters[renderingParameter.Name];

                if (!value.StartsWith("\"") && !value.StartsWith("'"))
                {
                    renderingParameter.Type = RenderingParameterType.Unresolved;
                }
                else
                {
                    renderingParameter.Type = RenderingParameterType.StringValue;
                }

                renderingParameter.ValueObject = value.Trim('"', '\'');
            }

            return renderingParameter;
        }
    }
}