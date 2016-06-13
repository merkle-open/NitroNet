using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine.Test
{
	[TestClass]
	public class FileSystemTest
	{
        protected PathInfo TestFileName1 = PathInfo.Create("test1.txt");
        protected PathInfo TestFileName2 = PathInfo.Create("test2.txt");
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
        public async Task TestWrite()
		{
            Assert.AreEqual(false, FileSystem.FileExists(TestFileName1));

			var completion = new TaskCompletionSource<IFileInfo>();
		    using (await FileSystem.SubscribeAsync(TestFilePattern, info => completion.SetResult(info)).ConfigureAwait(false))
		    {
                using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName1)))
                {
                    stream.Write("123456");
                }

                await completion.Task.ConfigureAwait(false);
            }

			Assert.AreEqual(true, FileSystem.FileExists(TestFileName1));

			FileSystem.RemoveFile(TestFileName1);
		}

		[TestMethod]
        [DeploymentItem("TestCases")]
        public async Task TestReWrite()
		{
            Assert.AreEqual(false, FileSystem.FileExists(TestFileName2));

            var completion = new TaskCompletionSource<IFileInfo>();
		    using (
		        await FileSystem.SubscribeAsync(TestFilePattern, info => completion.SetResult(info)).ConfigureAwait(false))
		    {
                using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName2)))
                {
                    stream.Write("123456");
                }

                await completion.Task.ConfigureAwait(false);
            }

            Assert.AreEqual(true, FileSystem.FileExists(TestFileName2));

            var completion2 = new TaskCompletionSource<IFileInfo>();
		    using (
		        await FileSystem.SubscribeAsync(TestFilePattern, info => completion2.SetResult(info)).ConfigureAwait(false)
		        )
		    {
                using (var stream = new StreamReader(FileSystem.OpenRead(TestFileName2)))
                {
                    Assert.AreEqual("123456", stream.ReadToEnd());
                }

                using (var stream = new StreamWriter(FileSystem.OpenWrite(TestFileName2)))
                {
                    stream.Write("654321");
                }

                await completion2.Task.ConfigureAwait(false);
            }
            
            Assert.AreEqual(true, FileSystem.FileExists(TestFileName2));

            var completion3 = new TaskCompletionSource<IFileInfo>();
		    using (
		        await FileSystem.SubscribeAsync(TestFilePattern, info => completion3.SetResult(info)).ConfigureAwait(false)
		        )
		    {
                using (var stream = new StreamReader(FileSystem.OpenRead(TestFileName2)))
                {
                    Assert.AreEqual("654321", stream.ReadToEnd());
                }

                FileSystem.RemoveFile(TestFileName2);

                await completion3.Task.ConfigureAwait(false);
            }

            try
            {
                using (var stream = new StreamReader(FileSystem.OpenRead(TestFileName2)))
                {
                }
                Assert.Fail();
            }
            catch (Exception)
            {
            }
        }
    }
}