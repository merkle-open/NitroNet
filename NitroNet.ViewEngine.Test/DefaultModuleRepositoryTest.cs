using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine.Test
{
    [TestClass]
    public class DefaultModuleRepositoryTest
    {
        [TestMethod]
        public void TestOnlyUseTemplatesFromModulePath()
        {
            var templateRepository = CreateRepository(
                new ComponentMockup {Id = "modules/Mod1/Mod1", Path = "modules/Mod1/Mod1.html", Type = TemplateType.Component},
                new ComponentMockup {Id = "modules/Mod2/mod2", Path = "modules/Mod2/mod2.html", Type = TemplateType.Component },
                new ComponentMockup {Id = "layouts/Layout1", Path = "layouts/Layout1.html", Type = TemplateType.View });

            var underTest = new DefaultComponentRepository(templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Mod1", result[0].Id);
            Assert.AreEqual("Mod2", result[1].Id);
        }

        [TestMethod]
        public void TestModuleContainsSkins()
        {
            var templateRepository = CreateRepository(
                new ComponentMockup { Id = "modules/Mod1/Mod1-skin1", Path = "modules/Mod1/Mod1-skin1.html", Type = TemplateType.Component },
                new ComponentMockup { Id = "modules/Mod1/Mod1-skin2", Path = "modules/Mod1/Mod1-skin2.html", Type = TemplateType.Component });

            var underTest = new DefaultComponentRepository(templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mod1", result[0].Id, "Module name should be Mod1");
            Assert.AreEqual(2, result[0].Skins.Count, "Two skins expected for module");
            Assert.IsNull(result[0].DefaultTemplate);
            Assert.IsTrue(result[0].Skins.ContainsKey("Mod1-skin1"), "Expected to have skin with name Mod1-skin1");
            Assert.IsTrue(result[0].Skins.ContainsKey("Mod1-skin2"), "Expected to have skin with name Mod1-skin2");
        }
        
        [TestMethod]
        public void TestModuleContainsUseDefaultTemplateWithSameName()
        {
            var templateRepository = CreateRepository(
                new ComponentMockup { Id = "modules/Mod1/Mod1", Path = "modules/Mod1/Mod1.html", Type = TemplateType.Component },
                new ComponentMockup { Id = "modules/Mod1/Mod1-skin1", Path = "modules/Mod1/Mod1-skin1.html", Type = TemplateType.Component },
                new ComponentMockup { Id = "modules/Mod1/Mod1-skin2", Path = "modules/Mod1/Mod1-skin2.html", Type = TemplateType.Component });

            var underTest = new DefaultComponentRepository(templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mod1", result[0].Id, "Module name should be Mod1");
            Assert.IsNotNull(result[0].DefaultTemplate);
            Assert.AreEqual("modules/Mod1/Mod1", result[0].DefaultTemplate.Id);
            Assert.AreEqual(2, result[0].Skins.Count, "Two skins expected for module");
        }

        [TestMethod]
        public void TestModuleContainsHypensIntheName()
        {
            var templateRepository = CreateRepository(
                new ComponentMockup { Id = "modules/Mod-1/Mod-1", Path = "modules/Mod-1/Mod-1.html", Type = TemplateType.Component },
                new ComponentMockup { Id = "modules/Mod-1/Mod-1-skin1", Path = "modules/Mod-1/Mod-1-skin1.html", Type = TemplateType.Component },
                new ComponentMockup { Id = "modules/Mod-1/Mod-1-skin2", Path = "modules/Mod-1/Mod-1-skin2.html", Type = TemplateType.Component });

            var underTest = new DefaultComponentRepository(templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mod-1", result[0].Id, "Module name should be Mod-1");
            Assert.IsNotNull(result[0].DefaultTemplate);
            Assert.AreEqual("modules/Mod-1/Mod-1", result[0].DefaultTemplate.Id);
            Assert.AreEqual(2, result[0].Skins.Count, "Two skins expected for module");
        }

        [TestMethod]
        public void TestModuleContainsUseDefaultTemplateWhenOnlyOneTemplate()
        {
            var templateRepository = CreateRepository(
                new ComponentMockup { Id = "test", Path = "modules/Mod1/test.html", Type = TemplateType.Component });

            var underTest = new DefaultComponentRepository(templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mod1", result[0].Id, "Module name should be Mod1");
            Assert.IsNotNull(result[0].DefaultTemplate);
            Assert.AreEqual("test", result[0].DefaultTemplate.Id);
            Assert.AreEqual("modules/Mod1/test.html", result[0].DefaultTemplate.Path.ToString());
            Assert.AreEqual(0, result[0].Skins.Count, "No skins expected for module");
        }

        [TestMethod]
        public void TestModuleContainsUseDefaultTemplateWhenOnlyOneTemplateWithoutSkin()
        {
            var templateRepository = CreateRepository(
                new ComponentMockup { Id = "modules/Mod1/test", Path = "modules/Mod1/test.html" },
                new ComponentMockup { Id = "modules/Mod1/test-skin", Path = "modules/Mod1/test-skin.html" });

            var underTest = new DefaultComponentRepository(templateRepository);
            var result = underTest.GetAll().ToList();

            Assert.AreEqual(0, result.Count);
        }

        private static ITemplateRepository CreateRepository(params ComponentMockup[] templates)
        {
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(f => f.Path).Returns(new FileSystem.FilePathHelper());

            var templateRepo = new Mock<ITemplateRepository>();
            templateRepo.Setup(t => t.GetAll()).Returns(templates.Select(template => new FileTemplateInfo(template.Id, template.Type, PathInfo.Create(template.Path), fileSystemMock.Object)));
            return templateRepo.Object;
        }

        private class ComponentMockup
        {
            public string Id { get; set; }
            public string Path { get; set; }
            public TemplateType Type { get; set; }
        }
    }
}
