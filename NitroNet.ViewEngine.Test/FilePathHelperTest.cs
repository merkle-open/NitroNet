using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine.Test
{
    [TestClass]
    public class FilePathHelperTest
    {
        [TestMethod]
        public void TestCombine_SingleItemReturnsSameItem()
        {
            TestCombine("in", "in");
        }

        [TestMethod]
        public void TestCombine_TwoPartsReturnsConcanatedPath()
        {
            TestCombine("test1/test2", "test1", "test2");
        }

        [TestMethod]
        public void TestCombine_TwoPartsReturnsConcanatedPathWithoutTrailingSlash()
        {
            TestCombine("test1/test2", "test1", "test2/");
        }

        [TestMethod]
        public void TestCombine_TwoPartsWithBackslashReturnsNormalizedPath()
        {
            TestCombine("test1/test2/test3", "test1", "test2\\test3\\");
        }

        [TestMethod]
        public void TestCombine_DotDotSyntaxReturnsParentPath()
        {
            TestCombine("test2/test3", "test1", "..\\test2\\test3\\");
        }

        [TestMethod]
        public void TestCombine_CombineTwoFirstWithTrailingSlash()
        {
            TestCombine("test1/test2", "test1/", "test2");
        }

        [TestMethod]
        public void TestCombine_TwoPathsOneRootedReturnsRootedPath()
        {
            TestCombine("/test1/test2", "/test1", "test2");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCombine_TwoRootedPathsReturnsThrowsException()
        {
            TestCombine("test2/test3", "/root1", "/root1");
        }

        [TestMethod]
        public void TestCombine_TwoPartsOneWithDot()
        {
            TestCombine("test1/test2/test3/test.txt", "test1", "test2\\test3\\test.txt");
        }

        [TestMethod]
        public void TestCombine_TwoOneReferencesSelf()
        {
            TestCombine("/test1/test2/test3/test.txt", "/test1", ".\\test2\\test3\\test.txt");
        }

        [TestMethod]
        public void TestCombine_CombinedPathWithDotDots()
        {
            TestCombine("test1/test2/test3/test.txt", "test1\\test_false\\../test2/test3/test.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCombine_ThrowsExceptionOnEmptyPart()
        {
            TestCombine("test1/test2", "test1/", "test2//soso");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCombine_TwoSecondRootedPathsReturnsThrowsException()
        {
            TestCombine("test1/test2", "test1/", "/test2");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCombine_DuplicatedPathSeperatorThrowsException()
        {
            TestCombine("test2/test3", "/root1", "root1//gugus");
        }

        private static void TestCombine(string expected, params string[] parameters)
        {
            var underTest = new FileSystem.FilePathHelper();
            var result = underTest.Combine(parameters.Select(PathInfo.Create).ToArray()).ToString();

            Assert.AreEqual(expected, result);
        }
    }
}
