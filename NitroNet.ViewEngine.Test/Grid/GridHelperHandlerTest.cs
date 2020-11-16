using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroNet.Veil.Handlers.Grid;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler.Grid;

namespace NitroNet.ViewEngine.Test.Grid
{
	[TestClass]
	public class GridHelperHandlerTest
	{
		[TestMethod]
		public void TestCalculation1_3()
		{
			var underTest = new GridHelperHandler();

			var renderingContext = new RenderingContext(null);
			var gridStack = GridStack.FromContext(renderingContext);
			gridStack.Push(960);

			underTest.EvaluateAsync(null, renderingContext, new Dictionary<string, string>
			{
			    {"ratio", "1/3"}
			});

			var result = gridStack.Current;
			Assert.AreEqual(320, result.Width);
		}
	}
}
