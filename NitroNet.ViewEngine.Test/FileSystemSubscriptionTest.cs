using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine.Test
{
	[TestClass]
	public class FileSystemSubscriptionTest
	{
		protected PathInfo TestFileName1 = PathInfo.Create("test.txt");
        protected PathInfo TestFileName2 = PathInfo.Create("test.txt");
        protected const string TestFilePattern = "*.txt";
        protected IFileSystem FileSystem;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public virtual void Init()
        {
            var config = ConfigurationLoader.LoadNitroConfiguration(TestContext.DeploymentDirectory);
            FileSystem = new FileSystem(config);
            FileSystem.RemoveFile(TestFileName1);
            FileSystem.RemoveFile(TestFileName2);
        }

        [TestMethod]
        [DeploymentItem("TestCases")]
        public async Task TestSubscription()
		{
            Assert.AreEqual(false, FileSystem.FileExists(TestFileName1));

			var c = new TaskCompletionSource<IFileInfo>();
			using (await FileSystem.SubscribeAsync(TestFilePattern, s => c.TrySetResult(s)).ConfigureAwait(false))
			{
				using (var writer = new StreamWriter(FileSystem.OpenWrite(TestFileName1)))
				{
					writer.BaseStream.SetLength(0);
					writer.Write("123456789");
				}

				var result = await c.Task.ConfigureAwait(false);
				Assert.AreEqual(TestFileName1.ToString(), Path.GetFileName(result.FilePath.ToString()));
			}
		}

		[TestMethod]
        [DeploymentItem("TestCases")]
        public async Task TestDirectorySubscription()
		{
            Assert.AreEqual(false, FileSystem.FileExists(TestFileName2));

			var c = new TaskCompletionSource<IEnumerable<IFileInfo>>();
			using (await FileSystem.SubscribeDirectoryGetFilesAsync(PathInfo.Create(""), "txt", infos =>
				{
					c.TrySetResult(infos);
				}).ConfigureAwait(false))
			{
				using (var writer = new StreamWriter(FileSystem.OpenWrite(TestFileName2)))
				{
					writer.BaseStream.SetLength(0);
					writer.Write("123456789");
				}

				var result = await c.Task.ConfigureAwait(false);
				Assert.AreEqual(TestFileName2.ToString(), Path.GetFileName(result.First().FilePath.ToString()));
			}
		}
	}
}