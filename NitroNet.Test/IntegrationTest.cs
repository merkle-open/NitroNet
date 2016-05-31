using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.ViewEngines;
using Veil;
using Veil.Helper;

namespace NitroNet.Test
{
	[TestClass]
	public class IntegrationTest
	{
		[TestMethod]
		public async Task EscapeNullString()
		{
			var cacheProvider = new MemoryCacheProvider();
			var handlerFactory = new Mock<IHelperHandlerFactory>();

			handlerFactory.Setup(f => f.Create()).Returns(Enumerable.Empty<IHelperHandler>());

			var namingRule = new NamingRule();
			const string input = "<p>{{{name}}}</p>";
			var templateInfo = new StringTemplateInfo("views/test", input);

			var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory.Object, namingRule);

			var view = await viewEngine.CreateViewAsync(templateInfo, typeof(TestModel)).ConfigureAwait(false);

			var model = new TestModel();

			var writer = new StringWriter();
			view.Render(model, new RenderingContext(writer));
			var stringResult = writer.ToString();

			Assert.AreEqual("<p></p>", stringResult);
		}

		[TestMethod]
		public async Task EscapeNullStringLateBinding()
		{
			var cacheProvider = new MemoryCacheProvider();
			var handlerFactory = new Mock<IHelperHandlerFactory>();

			handlerFactory.Setup(f => f.Create()).Returns(Enumerable.Empty<IHelperHandler>());

			var namingRule = new NamingRule();
			const string input = "<p>{{{name}}}</p>";
			var templateInfo = new StringTemplateInfo("views/test", input);

			var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory.Object, namingRule);

			var view = await viewEngine.CreateViewAsync(templateInfo, typeof(object)).ConfigureAwait(false);

			var model = new TestModel();

			var writer = new StringWriter();
			view.Render(model, new RenderingContext(writer));
			var stringResult = writer.ToString();

			Assert.AreEqual("<p></p>", stringResult);
		}

		private class TestModel
		{
			public string Name { get; set; }
		}

	}
}
