using System;
using System.Collections.Generic;
using System.Linq;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler;
using Veil.Helper;

namespace NitroNet.Veil.Handlers
{
    public class LabelHelperHandler : IHelperHandler
    {
        private readonly INitroTemplateHandler _handler;

        public LabelHelperHandler(INitroTemplateHandler handler)
        {
            _handler = handler;
        }

        public bool IsSupported(string name)
        {
            return name.Equals("t", StringComparison.OrdinalIgnoreCase);
        }

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var key = parameters.Keys.First().Trim('"', '\'');
			_handler.RenderLabel(key, context);
		}
    }
}