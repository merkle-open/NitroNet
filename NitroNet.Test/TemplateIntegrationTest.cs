using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NitroNet.Veil.ViewEngine;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.IO;
using Veil.Helper;

namespace NitroNet.Test
{
    [TestClass]
    public class TemplateIntegrationTest
    {
        private string _testName;
        private string _dataFile;
        private string _templateFile;
        private string _resultFile;
        private string _result;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            _dataFile = (string)TestContext.DataRow["data"];
            _templateFile = (string)TestContext.DataRow["template"];
            _resultFile = Path.Combine(this.TestContext.DeploymentDirectory, (string)TestContext.DataRow["result"]);
            _testName = Path.GetFileName(Path.GetDirectoryName(_resultFile));

            _result = GetFileContent(_resultFile);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\testcases\\test_cases.csv", "test_cases#csv", DataAccessMethod.Sequential)]
        [DeploymentItem("TestCases")]
        public async Task TestServerSideRendering()
        {
            var resultString = await ExecuteServerSide(_testName, _templateFile, _dataFile).ConfigureAwait(false);
            Assert.AreEqual(_result, resultString);
        }

        private static string GetFileContent(string resultFile)
        {
            string result;
            using (var reader = new StreamReader(resultFile))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private class StringWriterDelayed : StringWriter
        {
            public StringWriterDelayed(StringBuilder sb) : base(sb)
            {
            }

            public override async Task WriteAsync(string value)
            {
                await Task.Yield();
                //await Task.Delay(100);
                await base.WriteAsync(value);
            }

            public override async Task WriteAsync(char value)
            {
                await Task.Yield();
                //await Task.Delay(100);
                await base.WriteAsync(value);
            }
        }

        private void CleanupModel(Dictionary<string, object> model)
        {
            foreach (var entry in model.ToList())
            {
                var value = entry.Value;
                var list = value as ArrayList;
                if (list != null)
                {
                    var objs = list.OfType<object>().ToList();
                    model[entry.Key] = objs;
                    value = objs;

                    //continue;
                }

                var enumerable = value as IEnumerable<object>;
                if (enumerable != null)
                {
                    foreach (var item in enumerable)
                    {
                        var dItem = item as Dictionary<string, object>;
                        if (dItem != null)
                            CleanupModel(dItem);
                    }
                }

                var dict = value as Dictionary<string, object>;
                if (dict != null)
                    CleanupModel(dict);
            }
        }

        private class MyResolver : JavaScriptTypeResolver
        {
            public MyResolver()
            {
            }

            public override Type ResolveType(string id)
            {
                return typeof (Dictionary<string, object>);
            }

            public override string ResolveTypeId(Type type)
            {
                return type.Name;
            }
        }

        private async Task<string> ExecuteServerSide(string testName, string templateFile, string dataFile)
        {
            var cacheProvider = new MemoryCacheProvider();
            var namingRule = new NamingRule();
            var handlerFactory = new NullRenderingHelperHandlerFactory();
            var config = ConfigurationLoader.LoadNitroConfiguration(Path.Combine(TestContext.DeploymentDirectory, "Basic"));

            var templateInfo = new FileTemplateInfo(testName, TemplateType.View, PathInfo.Create(templateFile), new FileSystem(config));

            var viewEngine = new VeilViewEngine(cacheProvider, handlerFactory, namingRule);
            IView view = await viewEngine.CreateViewAsync(templateInfo).ConfigureAwait(false);
            if (view == null)
                Assert.Fail("Could not create view from file'{0}'.", templateFile);

            object model;
            using (var reader = new StreamReader(dataFile))
            {
                model = new JsonSerializer().Deserialize(reader, typeof(Dictionary<string, object>));
            }

            if (TestContext.Properties.Contains("$LoadTestUserContext"))
                this.TestContext.BeginTimer("Server");

            var builder = new StringBuilder();
            using (var writer = new StringWriterDelayed(builder))
            {
                view.Render(model, new RenderingContext(writer));
            }
            var resultString = builder.ToString();

            if (TestContext.Properties.Contains("$LoadTestUserContext"))
                this.TestContext.EndTimer("Server");

            return resultString;
        }

        private class NullRenderingHelperHandlerFactory : IHelperHandlerFactory
        {
            public IEnumerable<IHelperHandler> Create()
            {
                return Enumerable.Empty<IHelperHandler>();
            }
        }
    }
}
