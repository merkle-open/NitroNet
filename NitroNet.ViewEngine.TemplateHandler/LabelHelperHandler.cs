using System;
using System.Collections.Generic;
using System.Linq;
using Veil;
using Veil.Helper;

namespace NitroNet.ViewEngine.TemplateHandler
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