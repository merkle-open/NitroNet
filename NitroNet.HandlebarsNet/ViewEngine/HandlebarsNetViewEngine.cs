using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using NitroNet.Mvc;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Context;

namespace NitroNet.HandlebarsNet.ViewEngine
{
    public class HandlebarsNetViewEngine : IViewEngine
    {
        private readonly ICacheProvider _cacheProvider;
	    private readonly IHandlebarsNetEngine _engine;


        public HandlebarsNetViewEngine(IHandlebarsNetEngine engine, ICacheProvider cacheProvider)
		{
			_cacheProvider = cacheProvider;
		    _engine = engine;
		}

		public async Task<IView> CreateViewAsync(TemplateInfo templateInfo, Type modelType)
		{
			if (templateInfo == null)
				return null;

			var hash = string.Concat("template_", templateInfo.Id, templateInfo.ETag, modelType.FullName);

			IView view;
			if (!_cacheProvider.TryGet(hash, out view))
			{
				string content;
				using (var reader = new StreamReader(templateInfo.Open()))
				{
					content = await reader.ReadToEndAsync().ConfigureAwait(false);
				}

				if (modelType == typeof(object))
					view = CreateNonGenericView(templateInfo.Id, content, _engine);

				_cacheProvider.Set(hash, view, DateTimeOffset.Now.AddHours(24));
			}

			return view;
		}

		private static IView CreateNonGenericView(string templateId, string content, IHandlebarsNetEngine viewEngine)
		{
		    var render = viewEngine.Compile(new StringReader(content));
			return new HandlebarsNetViewAdapter<object>(templateId, new HandlebarsNetView<object>(render));
		}

        private class HandlebarsNetViewAdapter<T> : IView
		{
			private readonly string _templateId;
			private readonly IView<T> _adaptee;

			public HandlebarsNetViewAdapter(string templateId, IView<T> adaptee)
			{
				_templateId = templateId;
				_adaptee = adaptee;
			}

			public void Render(object model, RenderingContext context)
			{
				context.Data["templateId"] = _templateId;

				var castModel = model is T ? (T)model : default(T);
				if (castModel != null)
                {
                    //var updatedModel = ApplyContextToModel(castModel, context);

                    _adaptee.Render(castModel, context);
					return;
				}

				// TODO: Verify what is to be done with null model values
				if (typeof(T) == typeof(object))
				{
                    //var updatedModel = ApplyContextToModel((T)(object)new JObject(), context);

                    _adaptee.Render((T)(object)new JObject(), context);
					return;
				}

				_adaptee.Render(Activator.CreateInstance<T>(), context);
			}

            protected virtual dynamic ApplyContextToModel(T model, RenderingContext context)
            {
                IDictionary<string, object> updatedModel;

                if (model != null)
                {
                    updatedModel = CreateDictionary(model);
                }
                else
                {
                    updatedModel = new Dictionary<string, object>();
                }

                updatedModel[MvcRenderingContext.ContextKey] = context;

                return updatedModel;
            }

            private IDictionary<string, object> CreateDictionary(object model)
            {
                var comparer = StringComparer.InvariantCultureIgnoreCase;

                if (model is IDictionary sourceModel)
                {
                    var copiedModel = new Dictionary<string, object>(comparer);

                    foreach (var k in sourceModel.Keys)
                    {
                        copiedModel[k.ToString()] = sourceModel[k];
                    }

                    return copiedModel;
                }

                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;

                var dictionary = model.GetType().GetProperties(bindingAttr).ToDictionary
                (
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(model, null),
                    comparer
                );

                foreach (var fieldInfo in model.GetType().GetFields(bindingAttr))
                {
                    dictionary.Add(fieldInfo.Name, fieldInfo.GetValue(model));
                }

                return dictionary;
            }

        }

		private class HandlebarsNetView<T> : IView<T>
		{
			private readonly Action<TextWriter, T> _render;

			public HandlebarsNetView(Action<TextWriter, T> render)
			{
				_render = render;
			}

			public void Render(T model, RenderingContext context)
			{
                try
                {
                    _render(context.Writer, model);
                }
                catch (HandlebarsCompilerException ex)
                {
                    throw new HandlebarsCompilerException(string.Format("Error in view '{0}'", context.Data["templateId"]), ex);
                }
            }
		}
	}
}
