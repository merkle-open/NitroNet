using System.Collections.Generic;
using System.Threading.Tasks;
using Veil;
using Veil.Helper;

namespace NitroNet.ViewEngine.TemplateHandler
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
