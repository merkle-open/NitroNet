using System.IO;
using System.Web.Mvc;
using Veil;

namespace NitroNet.Mvc
{
    public class MvcRenderingContext : RenderingContext
    {
	    private const string ContextKey = "_mvcrenderingcontext";
	    public ViewContext ViewContext { get; private set; }
        public IViewDataContainer ViewDataContainer { get; private set; }

	    public MvcRenderingContext(ViewContext viewContext, IViewDataContainer viewDataContainer, TextWriter writer, RenderingContext parentContext) 
			: base(writer ?? viewContext.Writer, parentContext)
        {
            ViewContext = viewContext;
            ViewDataContainer = viewDataContainer;
        }

		internal static MvcRenderingContext Build(ViewContext viewContext, IViewDataContainer viewDataContainer, TextWriter writer)
		{
			var context = GetFromViewContext(viewContext);
            if (context != null)
            {
                return context;
            }

			MvcRenderingContext parentContext = null;
            if (viewContext.ParentActionViewContext != null)
            {
                parentContext = GetFromViewContext(viewContext.ParentActionViewContext);
            }

			context = new MvcRenderingContext(viewContext, viewDataContainer, writer, parentContext);
			viewContext.ViewData.Add(ContextKey, context);

			return context;
		}

	    public static MvcRenderingContext GetFromViewContext(ViewContext viewContext)
	    {
		    object contextObj;
		    MvcRenderingContext context = null;
            if (viewContext.ViewData.TryGetValue(ContextKey, out contextObj))
            {
                context = contextObj as MvcRenderingContext;
            }

		    return context;
	    }
    }
}