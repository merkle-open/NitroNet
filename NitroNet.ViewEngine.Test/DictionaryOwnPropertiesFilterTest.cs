using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroNet.ViewEngine.ViewEngines;

namespace NitroNet.ViewEngine.Test
{
    [TestClass]
    public class DictionaryOwnPropertiesFilterTest
    {
        [TestMethod]
        public void CorrectTypeSupported()
        {
            var uut = new DictionaryOwnPropertiesFilter();
            
            Assert.IsTrue(uut.IsTypeSupported(typeof(Dictionary<string, object>)));
            Assert.IsFalse(uut.IsTypeSupported(typeof(Dictionary<string, string>)));

        }

        [TestMethod]
        public void FilterBehaviourSuccess()
        {
            var uut = new DictionaryOwnPropertiesFilter();

            Assert.IsTrue(uut.IsValid("someproperty"));
            Assert.IsFalse(uut.IsValid("values"));
        }
    }
}
