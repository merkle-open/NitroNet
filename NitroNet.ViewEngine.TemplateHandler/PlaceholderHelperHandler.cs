using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using NitroNet.ViewEngine.TemplateHandler.RenderHandler;
using Veil;
using Veil.Helper;

namespace NitroNet.ViewEngine.TemplateHandler
{
	internal class PlaceholderHelperHandler : IHelperHandler
	{
		private readonly INitroTemplateHandler _handler;

		public PlaceholderHelperHandler(INitroTemplateHandler handler)
		{
			_handler = handler;
		}

		public bool IsSupported(string name)
		{
			return name.StartsWith("placeholder", StringComparison.OrdinalIgnoreCase);
		}

		public void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters)
		{
			var key = parameters["name"].Trim('"', '\'');
			var index = TryGetIndex(parameters, model);
			_handler.RenderPlaceholder(model, key, index, context);
		}

		private static string TryGetIndex(IDictionary<string, string> parameters, object model)
		{
            string fullIndex = null;

		    string index;
		    if (parameters.TryGetValue("index", out index))
		    {
		        fullIndex = index.Trim('"', '\'');
		    }

			string indexProperty;
		    if (parameters.TryGetValue("indexprop", out indexProperty))
		    {
		        indexProperty = indexProperty.Trim('"', '\'');

                string indexPropertyValue;
		        if (TryGetPropValue(model, indexProperty, out indexPropertyValue))
		        {
		            if (fullIndex == null)
		                return indexPropertyValue;

                    fullIndex += "_" + indexPropertyValue;
		        }
		    }

		    return fullIndex;
		}

		private static bool TryGetPropValue<TValue>(object src, string propertyName, out TValue value)
		{
			value = default(TValue);

			//JObject
			var jObject = src as JObject;
			JToken jValue;
			if (jObject != null && jObject.TryGetValue(propertyName, StringComparison.InvariantCultureIgnoreCase, out jValue))
			{
				value = jValue.Value<TValue>();
				return true;
			}

			//Dictionary
			var dictionaryObject = src as IDictionary<string, object>;
			object dictionaryValue;
			if (dictionaryObject != null && dictionaryObject.TryGetValue(propertyName, out dictionaryValue) && dictionaryValue is TValue)
			{
				value = (TValue)dictionaryValue;
				return true;
			}

			var property = src.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			if (property != null)
			{
				value = (TValue)property.GetValue(src, null);

				return true;
			}

			return false;
		}
	}
}