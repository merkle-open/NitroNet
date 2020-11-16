using System;
using System.Collections.Generic;
using System.Linq;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler;
using Veil.Helper;

namespace NitroNet.Veil.Handlers
{
	public class PartialHelperHandler : IHelperHandler
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