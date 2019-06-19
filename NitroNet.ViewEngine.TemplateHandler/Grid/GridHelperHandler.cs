using System.Collections.Generic;
using System.Threading.Tasks;
using Veil;
using Veil.Helper;

namespace NitroNet.ViewEngine.TemplateHandler.Grid
{
	public class GridHelperHandler : IBlockHelperHandler
	{
		private static readonly Dictionary<string, double> DefaultRatioTable = new Dictionary<string, double>
		{
			{"1/4", 0.25},
			{"1/2", 0.5},
			{"3/4", 0.75},
			{"1/3", (double)1/3},
			{"2/3", (double)2/3},
			{"1/5", (double)1/5}
		};

		public bool IsSupported(string name)
		{
			return name.StartsWith("grid-cell");
		}

		public Task EvaluateAsync(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			Evaluate(model, context, parameters);
		    return Task.FromResult(false);
		}

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var gridStack = GridStack.FromContext(context);
			double ratio = GetValue(parameters, "ratio", 1);
			double margin = GetValue(parameters, "margin", 0);
			double padding = GetValue(parameters, "padding", 0);
			double? componentPadding = GetValueNullable(parameters, "component-padding");
			double width = GetValue(parameters, "width", gridStack.Current.Width);

			gridStack.Push((int)(((width - margin) * ratio) - padding), componentPadding);
		}

		private static double GetValue(IDictionary<string, string> parameters, string key, double defaultValue)
		{
			double result = defaultValue;
			string value;
		    if (parameters.TryGetValue(key, out value) && !double.TryParse(value, out result) && !DefaultRatioTable.TryGetValue(value, out result))
		    {
		        result = defaultValue;
		    }
		    return result;
		}

		private static double? GetValueNullable(IDictionary<string, string> parameters, string key)
		{
			string value;
			if (parameters.TryGetValue(key, out value))
			{
				double result;
			    if (!double.TryParse(value, out result) && !DefaultRatioTable.TryGetValue(value, out result))
			    {
			        return result;
			    }
			}
			return null;
		}

		public void Leave(object model, RenderingContext context, string name, IDictionary<string, string> parameters)
		{
			var gridStack = GridStack.FromContext(context);
			gridStack.Pop();
		}
	}
}