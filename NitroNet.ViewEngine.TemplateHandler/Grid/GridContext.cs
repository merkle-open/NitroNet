using NitroNet.ViewEngine.Context;

namespace NitroNet.ViewEngine.TemplateHandler.Grid
{
	public class GridContext
	{
		public GridContext Parent { get; private set; }
		public double Width { get; private set; }
		public double ComponentPadding { get; private set; }

		internal GridContext(GridContext parent, double width, double? componentWidth = null)
		{
			Parent = parent;
			Width = width;
			ComponentPadding = componentWidth ?? (parent != null ? parent.ComponentPadding : 0);
		}

		public static GridContext GetFromRenderingContext(RenderingContext renderingContext)
		{
			return GridStack.FromContext(renderingContext).Current;
		}
	}
}