using System.Collections.Generic;
using System.Threading.Tasks;
using NitroNet.ViewEngine.Context;
using Veil.Helper;

namespace NitroNet.Veil.Handlers
{
    public class TemplateIdHelperHandler : IHelperHandler
    {
        public bool IsSupported(string name)
        {
            return name.StartsWith("template-id");
        }

        public Task EvaluateAsync(object model, RenderingContext context, IDictionary<string, string> parameters)
        {
            var templateId = context.Data["templateId"];
            return context.Writer.WriteAsync(templateId as string);
        }

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var templateId = context.Data["templateId"];
			context.Writer.Write(templateId as string);
		}
    }
}
