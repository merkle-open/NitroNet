using System.Collections.Generic;
using System.IO;
using System.Linq;
using HandlebarsDotNet.Compiler;
using NitroNet.HandlebarsNet.ViewEngine;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler;

namespace NitroNet.HandlebarsNet.Handlers
{
    public class HandlebarsNetComponentHandler : IHandlebarsNetHelperHandler
    {
        private readonly INitroTemplateHandler _handler;
        private readonly IRenderingContextFactory _renderingContextFactory;

        public HandlebarsNetComponentHandler(INitroTemplateHandler handler, IRenderingContextFactory renderingContextFactory)
        {
            _handler = handler;
            _renderingContextFactory = renderingContextFactory;
        }

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            var parametersAsDictionary = GetHashParameters(parameters);

            var renderingParameters = GetRenderingParameters(parametersAsDictionary, parameters);
            var renderingContext = _renderingContextFactory.GetOrCreate(output, context);

            _handler.RenderComponent(renderingParameters, context, renderingContext, parametersAsDictionary);
        }

        private HashParameterDictionary GetHashParameters(params object[] parameters)
        {
            HashParameterDictionary parametersAsDictionary = new HashParameterDictionary();

            foreach (var parameter in parameters)
            {
                if (parameter is HashParameterDictionary dictionary)
                {
                    parametersAsDictionary = dictionary;
                    break;
                }
            }

            return parametersAsDictionary;
        }

        protected RenderingParameter CreateRenderingParameter(string name, IDictionary<string, object> parameters)
        {
            var renderingParameter = new RenderingParameter(name);

            if (parameters.ContainsKey(renderingParameter.Name))
            {
                var value = parameters[renderingParameter.Name];

                renderingParameter.ValueObject = value;
                renderingParameter.Type = value is string ? RenderingParameterType.StringValue : RenderingParameterType.Resolved;
            }

            return renderingParameter;
        }

        private IDictionary<string, RenderingParameter> GetRenderingParameters(HashParameterDictionary hashParameters, params object[] parameters)
        {
            RenderingParameter template;
            var firstParameter = parameters.FirstOrDefault();

            if (firstParameter is string name)
            {
                template = new RenderingParameter(ComponentConstants.Name)
                {
                    ValueObject = name
                };
            }
            else
            {
                template = CreateRenderingParameter(ComponentConstants.Name, hashParameters);
            }

            var renderingParametersDictionary = new Dictionary<string, RenderingParameter>();

            renderingParametersDictionary.Add(ComponentConstants.Name, template);
            AddAdditionalRenderingParameters(hashParameters, renderingParametersDictionary);

            return renderingParametersDictionary;

        }

        protected virtual void AddAdditionalRenderingParameters(IDictionary<string, object> parameters, Dictionary<string, RenderingParameter> renderingParametersDictionary)
        {
            renderingParametersDictionary.Add(ComponentConstants.SkinParameter,
                CreateRenderingParameter(ComponentConstants.SkinParameter, parameters));
            renderingParametersDictionary.Add(ComponentConstants.DataParameter,
                CreateRenderingParameter(ComponentConstants.DataParameter, parameters));
        }
    }
}
