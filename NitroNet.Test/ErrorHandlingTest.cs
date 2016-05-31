using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.ViewEngines;
using Veil;
using Veil.Helper;
using Veil.Parser;

namespace NitroNet.Test
{
	[TestClass]
	public class ErrorHandlingTest
	{
		[TestMethod]
		public async Task TestPropertyNotBindable_ThrowsExpection()
		{
		    const string templateId = "views/test";
            const string input = "<p>{{   name  }}</p>";

		    try
		    {
		        await Execute(input, templateId, new object(), typeof(object));
		    }
		    catch (VeilCompilerException ex)
            {
                AssertException(ex, input, templateId, "name", 8);
                return;
            }
            Assert.Fail("Expected a VeilCompilerException");
		}

        [TestMethod]
        public async Task TestParseInvalidBlockStatementMissingEnd_ThrowsException()
        {
            const string templateId = "views/test";
            const string input = "<p>{{#if name}}</p>";

            try
            {
                await Execute(input, templateId, new object(), typeof(object));
            }
            catch (VeilParserException ex)
            {
                AssertException(ex, input, templateId, "</p>", 15);
                return;
            }
            Assert.Fail("Expected a VeilCompilerException");
        }

        [TestMethod]
        public async Task TestNullCollection_ThrowsException()
        {
            const string templateId = "views/test";
            const string input = "<p>{{#each items}}{{/each}}</p>";

            string output = string.Empty;

            try
            {
                output = await Execute(input, templateId, new Model1(), typeof(Model1));
            }
            catch (VeilCompilerException ex)
            {
                Assert.Fail("Didn't expect a VeilCompilerException.");
            }

            Assert.AreEqual("<p></p>", output);
        }

        [TestMethod]
        public async Task TestNullObject_ThrowsException()
        {
            const string templateId = "views/test";
            const string input = "<p>{{#if inner.inner.inner}}do{{/if}}</p>";

            try
            {
                await Execute(input, templateId, new Model1 { Inner = new Model1() }, typeof(Model1));
            }
            catch (VeilCompilerException ex)
            {
                AssertException(ex, input, templateId, "inner", 21);
                return;
            }
            Assert.Fail("Expected a VeilCompilerException");
        }

        private static void AssertException(VeilParserException ex, string input, string templateId, string enclosedText, int index)
        {
            Assert.IsNotNull(ex.Message);
            Assert.IsNotNull(ex.Location);
            AssertLocation(input, templateId, ex.Location, enclosedText, index);
        }

	    private static void AssertException(VeilCompilerException ex, string input, string templateId, string enclosedText, int index)
	    {
	        var innerException = ex.InnerException as VeilCompilerException;

            Assert.IsNotNull(innerException);
            Assert.IsNotNull(ex.Message);
	        Assert.IsNotNull(innerException.Node);
	        Assert.IsNotNull(innerException.Node.Location);
	        AssertLocation(input, templateId, innerException.Node.Location, enclosedText, index);
	    }

	    private static void AssertLocation(string input, string templateId, SourceLocation location, string enclosedText, int index)
	    {
	        Assert.AreEqual(enclosedText, input.Substring(location.Index, location.Length));
	        Assert.AreEqual(index, location.Index);
	        Assert.AreEqual(enclosedText.Length, location.Length);
	        Assert.AreEqual(templateId, location.TemplateId);
	    }

	    private static async Task<string> Execute(string input, string templateId, object model, Type modelType)
	    {
	        var cacheProvider = new MemoryCacheProvider();
	        var handlerFactory = new Mock<IHelperHandlerFactory>();

	        var namingRule = new NamingRule();
	        var templateInfo = new StringTemplateInfo(templateId, input);

	        var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory.Object, namingRule);

	        var view = await viewEngine.CreateViewAsync(templateInfo, modelType)
	            .ConfigureAwait(false);

	        var builder = new StringBuilder();
	        using (var writer = new StringWriter(builder))
	        {
	            var context = new RenderingContext(writer);
	            view.Render(model, context);
	        }
	        return builder.ToString();
	    }

	    private class Model1
	    {
	        public IList<string> Items { get; set; }
            public Model1 Inner { get; set; }
	    }
	}
}
