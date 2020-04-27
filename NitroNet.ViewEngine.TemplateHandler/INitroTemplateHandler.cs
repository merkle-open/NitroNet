using System.IO;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler
{
    public interface INitroTemplateHandler
    {
        void RenderPlaceholder(object model, string key, string index, RenderingContext context);
        void RenderPlaceholder(object model, string key, string index, TextWriter writer, ViewContext viewContext);

        void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model, ViewContext viewContext);
        
        [Obsolete(
            "Deprecated. Use method RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model,RenderingContext context, IDictionary<string, string> parameters) instead.")]        
        void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation,
            object model, RenderingContext context);

        void RenderComponent(IDictionary<string, RenderingParameter> renderingParameters, object model,
            RenderingContext context, IDictionary<string, string> parameters);

        void RenderLabel(string key, RenderingContext context);
		void RenderLabel(string key, ViewContext context);

        void RenderPartial(string template, object model, RenderingContext context);
        void RenderPartial(string template, object model, ViewContext context);
    }
}