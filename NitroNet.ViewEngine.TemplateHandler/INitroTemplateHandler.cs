using System;
using System.Collections.Generic;
using NitroNet.ViewEngine.Context;

namespace NitroNet.ViewEngine.TemplateHandler
{
    public interface INitroTemplateHandler
    {
        void RenderPlaceholder(object model, string key, string index, RenderingContext context);

        [Obsolete(
            "Deprecated. Use method RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model,RenderingContext context, IDictionary<string, string> parameters) instead.")]
        void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation,
            object model, RenderingContext context);

        void RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model,
            RenderingContext context, IDictionary<string, string> parameters);

        void RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model,
            RenderingContext context, IDictionary<string, object> parameters);

        void RenderLabel(string key, RenderingContext context);

        void RenderPartial(string template, object model, RenderingContext context);
    }
}