using System.Collections.Generic;
using System.IO;
using System.Linq;
using HandlebarsDotNet;
using HandlebarsDotNet.Compiler;
using NitroNet.HandlebarsNet.ViewEngine;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler.Grid;

namespace NitroNet.HandlebarsNet.Handlers.Grid
{
    public class HandlebarsNetGridCellHandler : IHandlebarsNetBlockHelperHandler
    {
        private readonly IRenderingContextFactory _renderingContextFactory;

        public HandlebarsNetGridCellHandler(IRenderingContextFactory renderingContextFactory)
        {
            _renderingContextFactory = renderingContextFactory;
        }

        private static readonly Dictionary<string, double> DefaultRatioTable = new Dictionary<string, double>
        {
            {"1/4", 0.25},
            {"1/2", 0.5},
            {"3/4", 0.75},
            {"1/3", (double)1/3},
            {"2/3", (double)2/3},
            {"1/5", (double)1/5}
        };

        public void Evaluate(TextWriter output, HelperOptions options, dynamic context, object[] parameters)
        {
            var parametersAsDictionary = (HashParameterDictionary)parameters.First();
            var renderingContext = _renderingContextFactory.GetOrCreate(output, context);

            var gridStack = GridStack.FromContext(renderingContext);
            double ratio = GetValue(parametersAsDictionary, "ratio", 1);
            double margin = GetValue(parametersAsDictionary, "margin", 0);
            double padding = GetValue(parametersAsDictionary, "padding", 0);
            double? componentPadding = GetValueNullable(parametersAsDictionary, "component-padding");
            double width = GetValue(parametersAsDictionary, "width", gridStack.Current.Width);

            gridStack.Push((int)(((width - margin) * ratio) - padding), componentPadding);
            options.Template(output, context);
            gridStack.Pop();
        }

        private static double GetValue(IDictionary<string, object> parameters, string key, double defaultValue)
        {
            double result = defaultValue;
            object value;
            if (parameters.TryGetValue(key, out value) && !double.TryParse(value.ToString(), out result) && !DefaultRatioTable.TryGetValue(value.ToString(), out result))
            {
                result = defaultValue;
            }
            return result;
        }

        private static double? GetValueNullable(IDictionary<string, object> parameters, string key)
        {
            object value;
            if (parameters.TryGetValue(key, out value))
            {
                double result;
                if (!double.TryParse(value.ToString(), out result) && !DefaultRatioTable.TryGetValue(value.ToString(), out result))
                {
                    return result;
                }
            }
            return null;
        }
    }
}
