using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NitroNet.Common.Exceptions;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.TemplateHandler.RenderHandler;

namespace NitroNet.ViewEngine.TemplateHandler.Test
{
    [TestClass]
    public class NitroTemplateHandlerUtilsTest
    {
        private const string SubpropertyValue = "result";
        private NitroTemplateHandlerUtils _nitroTemplateHandlerUtils;
        private Mock<INitroNetConfig> _nitroNetConfigMock;
        private EmptyDummyModel _emptyDummyModel;
        private DummyModel _dummyModel;
        private DummySubmodel _dummySubmodel;

        [TestInitialize]
        public void Setup()
        {
            _emptyDummyModel =  new EmptyDummyModel();
            _dummySubmodel = new DummySubmodel
            {
                Subproperty = SubpropertyValue
            };
            _dummyModel = new DummyModel
            {
                DummySubmodel = _dummySubmodel
            };
            var componentRepositoryMock = new Mock<IComponentRepository>();

            _nitroNetConfigMock = new Mock<INitroNetConfig>();

            _nitroNetConfigMock.Setup(config => config.EnableLiteralResolving).Returns(true);

            _nitroTemplateHandlerUtils = new NitroTemplateHandlerUtils(componentRepositoryMock.Object, _nitroNetConfigMock.Object);
        }

        [TestMethod]
        public void FindSubModelIsFalseOnMissingProperty()
        {
            var found = _nitroTemplateHandlerUtils.FindSubModel(new RenderingParameter("component") {Value = "dummysubmodel"},
                new RenderingParameter("skin"), new RenderingParameter("data"), new EmptyDummyModel(), null);

            Assert.IsFalse(found.SubModelFound);
            Assert.IsNull(found.Value);
        }

        [TestMethod]
        public void FindSubModelIsTrueOnSelfReference()
        {
            var result = new EmptyDummyModel();

            var found = _nitroTemplateHandlerUtils.FindSubModel(new RenderingParameter("component") { Value = "this" },
                new RenderingParameter("skin"), new RenderingParameter("data"), result, null);

            Assert.IsTrue(found.SubModelFound);
            Assert.AreEqual(result, found.Value);
        }

        [TestMethod]
        public void FindSubModelNoSuccessOnPropertyNotAvailable()
        {
            var result = new DummySubmodel();

            var found = _nitroTemplateHandlerUtils.FindSubModel(new RenderingParameter("component") { Value = "submodel2" },
                new RenderingParameter("skin"), new RenderingParameter("data"), new DummyModel { DummySubmodel = result }, null);

            Assert.IsFalse(found.SubModelFound);
            Assert.IsNull(found.Value);
        }

        [TestMethod]
        public void FindSubModelSuccessOnPropertyAvailable()
        {
            var utils = new NitroTemplateHandlerUtils(null, null);
            var result = new DummySubmodel();

            var found = utils.FindSubModel(new RenderingParameter("component") { Value = "dummysubmodel" },
                new RenderingParameter("skin"), new RenderingParameter("data"), new DummyModel {DummySubmodel = result}, null);
            Assert.IsTrue(found.SubModelFound);
            Assert.AreEqual(result, found.Value);
        }

        [TestMethod]
        public void FindSubModelSuccessOnSubpropertyAvailable()
        {
            var utils = new NitroTemplateHandlerUtils(null, null);

            var found = utils.FindSubModel(new RenderingParameter("component") {Value = "dummysubmodel.subproperty" },
                new RenderingParameter("skin"), new RenderingParameter("data"),
                new DummyModel {DummySubmodel = new DummySubmodel {Subproperty = SubpropertyValue}}, null);

            Assert.IsTrue(found.SubModelFound);
            Assert.AreEqual(SubpropertyValue, found.Value);
        }

        [TestMethod]
        public void GetPropertyValueFromObjectHierarchicallyDirect()
        {
            var success = _nitroTemplateHandlerUtils.GetPropertyValueFromObjectHierarchically(_dummyModel, "dummysubmodel",
                out var result);
            Assert.IsTrue(success);
            Assert.AreEqual(_dummySubmodel, result);
        }

        [TestMethod]
        public void GetPropertyValueFromObjectHierarchicallyNested()
        {
            var success = _nitroTemplateHandlerUtils.GetPropertyValueFromObjectHierarchically(_dummyModel, "dummysubmodel.subproperty",
                out var result);
            Assert.IsTrue(success);
            Assert.AreEqual(SubpropertyValue, result);
        }

        [TestMethod]
        public void GetPropertyValueFromObjectHierarchicallyNestedMissingProperty()
        {
            var success = _nitroTemplateHandlerUtils.GetPropertyValueFromObjectHierarchically(_dummyModel, "dummysubmodel.notexisting",
                out var result);
            Assert.IsFalse(success);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ResolveAdditionalParametersEnsureFlagCheck()
        {
            _nitroTemplateHandlerUtils.ResolveAdditionalParameters(_emptyDummyModel, new Dictionary<string, string>());
            _nitroNetConfigMock.Verify(config => config.EnableLiteralResolving, Times.Once);
        }

        [TestMethod]
        public void ResolveAdditionalParametersEnsureSkipDefaultKeys()
        {
            var additionalParameters = _nitroTemplateHandlerUtils.ResolveAdditionalParameters(_emptyDummyModel,
                new Dictionary<string, string>
                {
                    {"data", "test"}, {"name", "test"}, {"template", "test"}
                });
            Assert.IsNotNull(additionalParameters);
            Assert.IsTrue(!additionalParameters.Any());
        }

        [TestMethod]
        public void ResolveAdditionalParametersHandleSimpleTypes()
        {
            var additionalParameters = _nitroTemplateHandlerUtils.ResolveAdditionalParameters(_emptyDummyModel,
                new Dictionary<string, string>
                {
                    {"integer", "3"}, {"bool", "true"}, {"string", "\"doublequotes\""}, {"string2", "'singlequotes'"}
                });

            Assert.IsNotNull(additionalParameters);
            Assert.That.ContainsKey(additionalParameters, "integer");
            Assert.That.ContainsKey(additionalParameters, "bool");
            Assert.That.ContainsKey(additionalParameters, "string");

            Assert.AreEqual(typeof(int), additionalParameters["integer"].ValueType);
            Assert.IsInstanceOfType(additionalParameters["integer"].Value, typeof(int));
            Assert.AreEqual(3, additionalParameters["integer"].Value);

            Assert.AreEqual(typeof(bool), additionalParameters["bool"].ValueType);
            Assert.IsInstanceOfType(additionalParameters["bool"].Value, typeof(bool));
            Assert.AreEqual(true, additionalParameters["bool"].Value);

            Assert.AreEqual(typeof(string), additionalParameters["string"].ValueType);
            Assert.IsInstanceOfType(additionalParameters["string"].Value, typeof(string));
            Assert.AreEqual("doublequotes", additionalParameters["string"].Value);

            Assert.AreEqual(typeof(string), additionalParameters["string2"].ValueType);
            Assert.IsInstanceOfType(additionalParameters["string2"].Value, typeof(string));
            Assert.AreEqual("singlequotes", additionalParameters["string2"].Value);
        }

        [TestMethod]
        public void ResolveAdditionalParametersHandleNullAndUndefined()
        {
            var additionalParameters = _nitroTemplateHandlerUtils.ResolveAdditionalParameters(_emptyDummyModel,
                new Dictionary<string, string>
                {
                    {"null", "null"}, {"undefined", "undefined"}
                });

            Assert.IsNotNull(additionalParameters);
            Assert.That.ContainsKey(additionalParameters, "null");
            Assert.That.ContainsKey(additionalParameters, "undefined");

            Assert.AreEqual(typeof(object), additionalParameters["null"].ValueType);
            Assert.AreEqual(typeof(object), additionalParameters["undefined"].ValueType);

            Assert.AreEqual(null, additionalParameters["null"].Value);
            Assert.AreEqual(null, additionalParameters["undefined"].Value);
        }

        [TestMethod]
        public void ResolveAdditionalParametersHandleObject()
        {
            var additionalParameters = _nitroTemplateHandlerUtils.ResolveAdditionalParameters(_dummyModel,
                new Dictionary<string, string>
                {
                    {"object", "dummysubmodel"}, {"string", "dummysubmodel.subproperty"}, {"uninitialized", "dummysubmodel.uninitialized"}
                });

            Assert.IsNotNull(additionalParameters);
            Assert.That.ContainsKey(additionalParameters, "object");
            Assert.That.ContainsKey(additionalParameters, "string");
            Assert.That.ContainsKey(additionalParameters, "uninitialized");

            Assert.AreEqual(typeof(DummySubmodel), additionalParameters["object"].ValueType);
            Assert.AreEqual(typeof(string), additionalParameters["string"].ValueType);
            Assert.AreEqual(typeof(object), additionalParameters["uninitialized"].ValueType);

            Assert.IsInstanceOfType(additionalParameters["object"].Value, typeof(DummySubmodel));
            Assert.IsInstanceOfType(additionalParameters["string"].Value, typeof(string));

            Assert.AreEqual(_dummySubmodel, additionalParameters["object"].Value);
            Assert.AreEqual(SubpropertyValue, additionalParameters["string"].Value);
            Assert.AreEqual(null, additionalParameters["uninitialized"].Value);
        }

        [TestMethod]
        public void ResolveAdditionalParametersThrowsExceptionOnMissingProperty()
        {
            Assert.ThrowsException<NitroNetComponentException>(() =>
                _nitroTemplateHandlerUtils.ResolveAdditionalParameters(_dummyModel,
                    new Dictionary<string, string> {{"object", "missing"}}));
        }

        [TestMethod]
        public void ApplyResolvedParametersEnsureFlagCheck()
        {
            _nitroTemplateHandlerUtils.ApplyResolvedParameters(_emptyDummyModel, new Dictionary<string, ResolvedParameter>());
            _nitroNetConfigMock.Verify(config => config.EnableLiteralResolving, Times.Once);
        }

        [TestMethod]
        public void ApplyResolvedParametersThrowsExceptionOnMissingProperty()
        {
            Assert.ThrowsException<NitroNetComponentException>(() =>
                _nitroTemplateHandlerUtils.ApplyResolvedParameters(_dummySubmodel,
                    new Dictionary<string, ResolvedParameter> {{"missing", new ResolvedParameter()}}));
        }

        [TestMethod]
        public void ApplyResolvedParametersThrowsExceptionOnTypeMismatch()
        {
            Assert.ThrowsException<NitroNetComponentException>(() =>
                _nitroTemplateHandlerUtils.ApplyResolvedParameters(_dummySubmodel,
                    new Dictionary<string, ResolvedParameter>
                        {{"subproperty", new ResolvedParameter {Value = 3, ValueType = typeof(int)}}}));
        }

        [TestMethod]
        public void ApplyResolvedParametersSimpleType()
        {
            _nitroTemplateHandlerUtils.ApplyResolvedParameters(_dummySubmodel,
                new Dictionary<string, ResolvedParameter>
                    {{"uninitialized", new ResolvedParameter {Value = SubpropertyValue, ValueType = typeof(string)}}});

            Assert.AreEqual(SubpropertyValue, _dummySubmodel.Uninitialized);
        }

        [TestMethod]
        public void ApplyResolvedParametersObject()
        {
            var overwrite = new DummySubmodel();
            _nitroTemplateHandlerUtils.ApplyResolvedParameters(_dummyModel,
                new Dictionary<string, ResolvedParameter>
                    {{"dummysubmodel", new ResolvedParameter {Value = overwrite, ValueType = typeof(DummySubmodel)}}});

            Assert.AreEqual(overwrite, _dummyModel.DummySubmodel);
        }

        [TestMethod]
        public void ApplyResolvedParametersDefault()
        {
            _nitroTemplateHandlerUtils.ApplyResolvedParameters(_dummySubmodel,
                new Dictionary<string, ResolvedParameter>
                {
                    {"someinteger", new ResolvedParameter {Value = null, ValueType = typeof(object)}},
                    {"somebool", new ResolvedParameter {Value = null, ValueType = typeof(object)}},
                    {"uninitialized", new ResolvedParameter {Value = null, ValueType = typeof(object)}},
                    {"SomeObject", new ResolvedParameter {Value = null, ValueType = typeof(object)}},
                });

            Assert.AreEqual(string.Empty, _dummySubmodel.Uninitialized);
            Assert.AreEqual(false, _dummySubmodel.SomeBool);
            Assert.AreEqual(0, _dummySubmodel.SomeInteger);
            Assert.AreEqual(null, _dummySubmodel.SomeObject);
        }

        [TestMethod]
        public void CleanNameEmpty()
        {
            var result = _nitroTemplateHandlerUtils.CleanName("");
            
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void CleanNameNull()
        {
            var result = _nitroTemplateHandlerUtils.CleanName(null);
            
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void CleanNameSuccess()
        {
            var result = _nitroTemplateHandlerUtils.CleanName("string with space and - and capital A");
            
            Assert.AreEqual("stringwithspaceandandcapitala", result);
        }

        private class EmptyDummyModel
        {
            
        }
        private class DummyModel
        {
            public DummySubmodel DummySubmodel { get; set; }
        }

        private class DummySubmodel
        {
            public string Subproperty { get; set; }

            public int SomeInteger { get; set; }
            public bool SomeBool { get; set; }
            public object SomeObject { get; set; }
            public string Uninitialized { get; set; }
        }
    }

    public static class AssertExtensions
    {
        public static void ContainsKey<TKey, TValue>(this Assert assert, IDictionary<TKey, TValue> target, TKey key)
        {
            Assert.IsTrue(target.ContainsKey(key));
        }
    }
}
