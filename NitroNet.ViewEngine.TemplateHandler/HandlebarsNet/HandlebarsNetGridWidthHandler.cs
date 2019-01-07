using NitroNet.ViewEngine.TemplateHandler.HandlebarsNet.Grid;
using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet
{
    internal abstract class HandlebarsNetBaseGridWidthHandler : IHandlebarsNetHelperHandler
    {
        internal abstract double GetWidth(HandlebarsNetGridStack gridStack);

        private int CalculateWidth(ViewContext context, IDictionary<string, object> parameters)
        {
            double ratio = 1.0;
            object ratioValue;
            if (parameters.TryGetValue("ratio", out ratioValue) && !double.TryParse(ratioValue.ToString(), out ratio))
            {
                ratio = 1.0;
            }

            var gridStack = HandlebarsNetGridStack.FromContext(context);
            return (int)(GetWidth(gridStack) * ratio);
        }

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            var viewContext = Sitecore.Mvc.Common.ContextService.Get().GetInstances<ViewContext>();

            var value = CalculateWidth(context, viewContext.First().ViewData as IDictionary<string, object>);
            context.Writer.Write(value.ToString());
        }
    }

    internal class HandlebarsNetGridWidthHandler : HandlebarsNetBaseGridWidthHandler
    {
        internal override double GetWidth(HandlebarsNetGridStack gridStack)
        {
            return gridStack.Current.Width;
        }
    }

    internal class HandlebarsNetGridComponentWidthHandler : HandlebarsNetBaseGridWidthHandler
    {
        internal override double GetWidth(HandlebarsNetGridStack gridStack)
        {
            return gridStack.Current.Width - gridStack.Current.ComponentPadding * 2;
        }
    }
}
