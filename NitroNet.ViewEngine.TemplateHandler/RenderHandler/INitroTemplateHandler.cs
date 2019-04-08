using System.Collections.Generic;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler.RenderHandler
{
    public interface INitroTemplateHandler
    {
        void RenderPlaceholder(object model, string key, string index, RenderingContext context);

        void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model, RenderingContext context, IDictionary<string, string> parameters);
        
		void RenderLabel(string key, RenderingContext context);
        
		void RenderPartial(string template, object model, RenderingContext context);
    }
}