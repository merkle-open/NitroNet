using System.Web.Mvc;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet.Grid
{
    internal class HandlebarsNetGridContext
    {
        public HandlebarsNetGridContext Parent { get; private set; }
        public double Width { get; private set; }
        public double ComponentPadding { get; private set; }

        internal HandlebarsNetGridContext(HandlebarsNetGridContext parent, double width, double? componentWidth = null)
        {
            Parent = parent;
            Width = width;
            ComponentPadding = componentWidth ?? (parent != null ? parent.ComponentPadding : 0);
        }

        public static HandlebarsNetGridContext GetFromRenderingContext(ViewContext viewContext)
        {
            return HandlebarsNetGridStack.FromContext(viewContext).Current;
        }
    }
}
