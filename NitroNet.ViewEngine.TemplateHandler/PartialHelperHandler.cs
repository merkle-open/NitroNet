using System;
using System.Collections.Generic;
using Veil;
using Veil.Helper;

namespace NitroNet.ViewEngine.TemplateHandler
{
	internal class PartialHelperHandler : IHelperHandler
	{
		private readonly INitroTemplateHandler _handler;

		public PartialHelperHandler(INitroTemplateHandler handler)
		{
			_handler = handler;
		}

		public bool IsSupported(string name)
		{
			return name.StartsWith("partial", StringComparison.OrdinalIgnoreCase);
		}

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var template = parameters["name"].Trim('"', '\'');
			_handler.RenderPartial(template, model, context);
		}
	}
}