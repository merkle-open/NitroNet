using System.Collections;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NitroNet.ViewEngine.Context;

namespace NitroNet.Mvc.Context
{
    public class MvcRenderingContextFactory : IRenderingContextFactory
    {
        public RenderingContext Create(TextWriter writer, object model)
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new FakeController());
            var viewContext = new ViewContext(controllerContext, new FakeView(), new ViewDataDictionary(model), new TempDataDictionary(), writer);
            var viewDataContainer = new FakeViewDataContainer
            {
                ViewData = viewContext.ViewData
            };

            return new MvcRenderingContext(viewContext, viewDataContainer, writer, null);
        }

        public RenderingContext GetOrCreate(TextWriter writer, object model)
        {
            if (model is IDictionary modelDictionary && modelDictionary.Contains(MvcRenderingContext.ContextKey))
            {
                return modelDictionary[MvcRenderingContext.ContextKey] as RenderingContext;
            }
            
            return Create(writer, model);
        }

        public class FakeController : Controller
        {
        }

        public class FakeViewDataContainer : IViewDataContainer
        {
            public ViewDataDictionary ViewData { get; set; }
        }

        public class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
