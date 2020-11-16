using System.Collections.Generic;
using System.Threading.Tasks;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using Veil.Helper;

namespace NitroNet.Veil.Handlers.Grid
{
	public abstract class BaseGridWidthHelperHandler : IHelperHandler
	{
	    public abstract bool IsSupported(string name);

		internal abstract double GetWidth(GridStack gridStack);

		public Task EvaluateAsync(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var value = CalculateWidth(context, parameters);
		    return context.Writer.WriteAsync(value.ToString());
		}

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var value = CalculateWidth(context, parameters);
			context.Writer.Write(value.ToString());
		}

		private int CalculateWidth(RenderingContext context, IDictionary<string, string> parameters)
		{
			double ratio = 1.0;
			string ratioValue;
		    if (parameters.TryGetValue("ratio", out ratioValue) && !double.TryParse(ratioValue, out ratio))
		    {
		        ratio = 1.0;
		    }

		    var gridStack = GridStack.FromContext(context);
			return (int)(GetWidth(gridStack) * ratio);
		}
	}

	public class GridWidthHelperHandler : BaseGridWidthHelperHandler
	{
		public override bool IsSupported(string name)
		{
			return name.StartsWith("grid-width");
		}

		internal override double GetWidth(GridStack gridStack)
		{
			return gridStack.Current.Width;
		}
	}

	public class GridComponentWidthHelperHandler : BaseGridWidthHelperHandler
	{
		public override bool IsSupported(string name)
		{
			return name.StartsWith("grid-component-width");
		}

		internal override double GetWidth(GridStack gridStack)
		{
			return gridStack.Current.Width - gridStack.Current.ComponentPadding * 2;
		}
	}
}