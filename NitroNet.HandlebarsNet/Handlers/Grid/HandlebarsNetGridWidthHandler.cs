using System.Collections.Generic;
using System.IO;
using NitroNet.HandlebarsNet.ViewEngine;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler.Grid;

namespace NitroNet.HandlebarsNet.Handlers.Grid
{
    public abstract class HandlebarsNetBaseGridWidthHandler : IHandlebarsNetHelperHandler
    {
        private readonly IRenderingContextFactory _renderingContextFactory;
        internal abstract double GetWidth(GridStack gridStack);

        protected HandlebarsNetBaseGridWidthHandler(IRenderingContextFactory renderingContextFactory)
        {
            _renderingContextFactory = renderingContextFactory;
        }

        private int CalculateWidth(RenderingContext context, IDictionary<string, object> parameters)
        {
            double ratio = 1.0;
            object ratioValue;
            if (parameters.TryGetValue("ratio", out ratioValue) && !double.TryParse(ratioValue.ToString(), out ratio))
            {
                ratio = 1.0;
            }

            var gridStack = GridStack.FromContext(context);
            return (int)(GetWidth(gridStack) * ratio);
        }

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            var renderingContext = _renderingContextFactory.GetOrCreate(output, context);

            if (renderingContext == null)
            {
                return;
            }

            var value = CalculateWidth(context, renderingContext.Data);
            context.Writer.Write(value.ToString());
        }
    }

    public class HandlebarsNetGridWidthHandler : HandlebarsNetBaseGridWidthHandler
    {
        internal override double GetWidth(GridStack gridStack)
        {
            return gridStack.Current.Width;
        }

        public HandlebarsNetGridWidthHandler(IRenderingContextFactory renderingContextFactory) : base(renderingContextFactory)
        {
        }
    }

    public class HandlebarsNetGridComponentWidthHandler : HandlebarsNetBaseGridWidthHandler
    {
        internal override double GetWidth(GridStack gridStack)
        {
            return gridStack.Current.Width - gridStack.Current.ComponentPadding * 2;
        }

        public HandlebarsNetGridComponentWidthHandler(IRenderingContextFactory renderingContextFactory) : base(renderingContextFactory)
        {
        }
    }
}
