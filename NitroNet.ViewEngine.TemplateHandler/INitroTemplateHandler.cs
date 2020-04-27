using System.IO;
using System.Web.Mvc;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler
{
    public interface INitroTemplateHandler
    {
        void RenderPlaceholder(object model, string key, string index, RenderingContext context);
        void RenderPlaceholder(object model, string key, string index, TextWriter writer, ViewContext viewContext);

        void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model, RenderingContext context);
        void RenderComponent(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model, ViewContext viewContext);
        
		void RenderLabel(string key, RenderingContext context);
		void RenderLabel(string key, ViewContext context);
        
		void RenderPartial(string template, object model, RenderingContext context);
        void RenderPartial(string template, object model, ViewContext context);
    }
}