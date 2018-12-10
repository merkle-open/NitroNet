using Newtonsoft.Json.Linq;
using NitroNet.ViewEngine.Cache;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HandlebarsDotNet;
using NitroNet.ViewEngine.Config;
using Veil;
using Veil.Compiler;
using Veil.Helper;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public class HandlebarsNetViewEngine : IViewEngine
	{
		private readonly ICacheProvider _cacheProvider;

		public HandlebarsNetViewEngine(ICacheProvider cacheProvider)
		{
			_cacheProvider = cacheProvider;
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

				var viewEngine = new HandlebarsNetEngine();
				if (modelType == typeof(object))
					view = CreateNonGenericView(templateInfo.Id, content, viewEngine);

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
					_adaptee.Render(castModel, context);
					return;
				}

				// TODO: Verify what is to be done with null model values
				if (typeof(T) == typeof(object))
				{
					_adaptee.Render((T)(object)new JObject(), context);
					return;
				}

				_adaptee.Render(Activator.CreateInstance<T>(), context);
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
                catch (VeilCompilerException ex)
                {
                    throw new VeilCompilerException(string.Format("Error in view '{0}'", context.Data["templateId"]), ex, null);
                }
            }
		}
	}
}
