using System;
using System.Collections.Generic;
using System.Linq;
using NitroNet.ViewEngine.TemplateHandler.RenderHandler;
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
		    var isSupported = name.StartsWith("partial", StringComparison.OrdinalIgnoreCase) || name.StartsWith(">", StringComparison.OrdinalIgnoreCase);
		    return isSupported;
		}

        public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
        { 
            var template = parameters.TryGetValue("name", out var value)
                ? value.Trim('"', '\'')
                : parameters.First().Key.Trim('"', '\'');
            _handler.RenderPartial(template, model, context);
        }
    }
}