using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NitroNet.Common.Exceptions;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.TemplateHandler.Models;
using NitroNet.ViewEngine.TemplateHandler.Utils;

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
        private static readonly HashSet<string> defaultKeys = new HashSet<string> { "name", "template", "data" };

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

            _nitroNetConfigMock.Setup(config => config.AdditionalArgumentsParsingMode).Returns(AdditionalArgumentsParsingMode.Full);
            _nitroNetConfigMock.Setup(config => config.EnableAdditionalArgumentsOnly).Returns(false);

            _nitroTemplateHandlerUtils = new NitroTemplateHandlerUtils(componentRepositoryMock.Object, _nitroNetConfigMock.Object);
        }

        private IDictionary<string, RenderingParameter> CreateRenderingParameters(string component, string skin, string data)
        {
            return new Dictionary<string, RenderingParameter>
            {
                {ComponentConstants.Name, new RenderingParameter("component") {Value = component}},
                {ComponentConstants.SkinParameter, new RenderingParameter("skin") {Value = skin}},
                {ComponentConstants.DataParameter, new RenderingParameter("data") {Value = data}}
            };
        }

        [TestMethod]
        public void FindSubModelIsFalseOnMissingProperty()
        {
            var found = _nitroTemplateHandlerUtils.FindSubModel(CreateRenderingParameters("dummysubmodel","",""), new EmptyDummyModel(), null);

            Assert.IsFalse(found.SubModelFound);
            Assert.IsNull(found.Value);
        }

        [TestMethod]
        public void FindSubModelIsTrueOnSelfReference()
        {
            var result = new EmptyDummyModel();

            var found = _nitroTemplateHandlerUtils.FindSubModel(CreateRenderingParameters("dummysubmodel", "", "this"), result, null);

            Assert.IsTrue(found.SubModelFound);
            Assert.AreEqual(result, found.Value);
        }

        [TestMethod]
        public void FindSubModelNoSuccessOnPropertyNotAvailable()
        {
            var result = new DummySubmodel();

            var found = _nitroTemplateHandlerUtils.FindSubModel(CreateRenderingParameters("submodel2", "", ""), new DummyModel { DummySubmodel = result }, null);

            Assert.IsFalse(found.SubModelFound);
            Assert.IsNull(found.Value);
        }

        [TestMethod]
        public void FindSubModelSuccessOnPropertyAvailable()
        {
            var utils = new NitroTemplateHandlerUtils(null, null);
            var result = new DummySubmodel();

            var found = utils.FindSubModel(CreateRenderingParameters("dummysubmodel", "", ""), new DummyModel {DummySubmodel = result}, null);
            Assert.IsTrue(found.SubModelFound);
            Assert.AreEqual(result, found.Value);
        }

        [TestMethod]
        public void FindSubModelSuccessOnSubpropertyAvailable()
        {
            var utils = new NitroTemplateHandlerUtils(null, null);
            
            var found = utils.FindSubModel(CreateRenderingParameters("dummysubmodel.subproperty", "", ""),
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
            _nitroTemplateHandlerUtils.ResolveAdditionalArguments(_emptyDummyModel, new Dictionary<string, string>(), defaultKeys);
            _nitroNetConfigMock.Verify(config => config.AdditionalArgumentsParsingMode, Times.Once);
        }

        [TestMethod]
        public void ResolveAdditionalParametersEnsureSkipDefaultKeys()
        {
            var additionalParameters = _nitroTemplateHandlerUtils.ResolveAdditionalArguments(_emptyDummyModel,
                new Dictionary<string, string>
                {
                    {"data", "test"}, {"name", "test"}, {"template", "test"}
                }, defaultKeys);
            Assert.IsNotNull(additionalParameters);
            Assert.IsTrue(!additionalParameters.Any());
        }

        [TestMethod]
        public void ResolveAdditionalParametersHandleSimpleTypes()
        {
            var additionalParameters = _nitroTemplateHandlerUtils.ResolveAdditionalArguments(_emptyDummyModel,
                new Dictionary<string, string>
                {
                    {"integer", "3"}, {"bool", "true"}, {"string", "\"doublequotes\""}, {"string2", "'singlequotes'"}
                }, defaultKeys);

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
            var additionalParameters = _nitroTemplateHandlerUtils.ResolveAdditionalArguments(_emptyDummyModel,
                new Dictionary<string, string>
                {
                    {"null", "null"}, {"undefined", "undefined"}
                }, defaultKeys);

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
            var additionalParameters = _nitroTemplateHandlerUtils.ResolveAdditionalArguments(_dummyModel,
                new Dictionary<string, string>
                {
                    {"object", "dummysubmodel"}, {"string", "dummysubmodel.subproperty"}, {"uninitialized", "dummysubmodel.uninitialized"}
                }, defaultKeys);

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
                _nitroTemplateHandlerUtils.ResolveAdditionalArguments(_dummyModel,
                    new Dictionary<string, string> {{"object", "missing"}}, defaultKeys));
        }

        [TestMethod]
        public void ApplyResolvedParametersEnsureFlagCheck()
        {
            _nitroTemplateHandlerUtils.ApplyResolvedArgumentsToObject(_emptyDummyModel, new Dictionary<string, ResolvedAdditionalArgument>());
            _nitroNetConfigMock.Verify(config => config.AdditionalArgumentsParsingMode, Times.Once);
        }

        [TestMethod]
        public void ApplyResolvedParametersThrowsExceptionOnMissingProperty()
        {
            Assert.ThrowsException<NitroNetComponentException>(() =>
                _nitroTemplateHandlerUtils.ApplyResolvedArgumentsToObject(_dummySubmodel,
                    new Dictionary<string, ResolvedAdditionalArgument> {{"missing", new ResolvedAdditionalArgument()}}));
        }

        [TestMethod]
        public void ApplyResolvedParametersThrowsExceptionOnTypeMismatch()
        {
            Assert.ThrowsException<NitroNetComponentException>(() =>
                _nitroTemplateHandlerUtils.ApplyResolvedArgumentsToObject(_dummySubmodel,
                    new Dictionary<string, ResolvedAdditionalArgument>
                        {{"subproperty", new ResolvedAdditionalArgument {Value = 3, ValueType = typeof(int)}}}));
        }

        [TestMethod]
        public void ApplyResolvedParametersSimpleType()
        {
            _nitroTemplateHandlerUtils.ApplyResolvedArgumentsToObject(_dummySubmodel,
                new Dictionary<string, ResolvedAdditionalArgument>
                    {{"uninitialized", new ResolvedAdditionalArgument {Value = SubpropertyValue, ValueType = typeof(string)}}});

            Assert.AreEqual(SubpropertyValue, _dummySubmodel.Uninitialized);
        }

        [TestMethod]
        public void ApplyResolvedParametersObject()
        {
            var overwrite = new DummySubmodel();
            _nitroTemplateHandlerUtils.ApplyResolvedArgumentsToObject(_dummyModel,
                new Dictionary<string, ResolvedAdditionalArgument>
                    {{"dummysubmodel", new ResolvedAdditionalArgument {Value = overwrite, ValueType = typeof(DummySubmodel)}}});

            Assert.AreEqual(overwrite, _dummyModel.DummySubmodel);
        }

        [TestMethod]
        public void ApplyResolvedParametersDefault()
        {
            _nitroTemplateHandlerUtils.ApplyResolvedArgumentsToObject(_dummySubmodel,
                new Dictionary<string, ResolvedAdditionalArgument>
                {
                    {"someinteger", new ResolvedAdditionalArgument {Value = null, ValueType = typeof(object)}},
                    {"somebool", new ResolvedAdditionalArgument {Value = null, ValueType = typeof(object)}},
                    {"uninitialized", new ResolvedAdditionalArgument {Value = null, ValueType = typeof(object)}},
                    {"SomeObject", new ResolvedAdditionalArgument {Value = null, ValueType = typeof(object)}},
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

        [TestMethod]
        public void IsValidSuccess()
        {
            var result = _nitroTemplateHandlerUtils.IsValid(new SubModel
            {
                PropertyName = "some",
                SubModelFound = true,
                Value = new { }
            });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidFailOnNotFoundOrValueNull()
        {
            var subModelNull = _nitroTemplateHandlerUtils.IsValid(new SubModel
            {
                PropertyName = "some",
                SubModelFound = true,
                Value = null
            });
            var noSubModelFound = _nitroTemplateHandlerUtils.IsValid(new SubModel
            {
                PropertyName = "some",
                SubModelFound = false,
                Value = null
            });

            Assert.IsFalse(subModelNull);
            Assert.IsFalse(noSubModelFound);
        }

        [TestMethod]
        public void IsValidFailOnString()
        {
            var submodelIsString = _nitroTemplateHandlerUtils.IsValid(new SubModel
            {
                PropertyName = "some",
                SubModelFound = true,
                Value = "astring"
            });
            Assert.IsFalse(submodelIsString);
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
