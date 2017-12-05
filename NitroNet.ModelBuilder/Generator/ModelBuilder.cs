using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Hosting;
using NitroNet.ModelBuilder.Generator.Models;
using NitroNet.ModelBuilder.Generator.Schema;
using NitroNet.ModelBuilder.Generator.Utilities;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace NitroNet.ModelBuilder.Generator
{
    public class ModelBuilder : IModelBuilder
    {
        private readonly INitroNetConfig _nitronetConfig;
        private readonly string _basePath;
        private readonly string _modelNamespace;
        private readonly bool _generateSingleFile;
        private readonly string _absoluteGenerationPath;

        public bool GenerateSingleFile => _generateSingleFile;

        public ModelBuilder(string basePath, INitroNetConfig configuration)
        {
            var relativeGenerationPath = ConfigurationManager.AppSettings["NitroNet.Generation.Path"] ?? "Models";
            var generateSingleFileString = ConfigurationManager.AppSettings["NitroNet.Generation.SingleFile"] ?? "true";

            _nitronetConfig = configuration;
            _basePath = basePath;
            _modelNamespace = ConfigurationManager.AppSettings["NitroNet.ModelNamespace"] ?? "NitroNet.Models";
            _generateSingleFile = !generateSingleFileString.Equals("false", StringComparison.InvariantCultureIgnoreCase);

            // TODO: Allow also absolute paths
            _absoluteGenerationPath = PathInfo.Combine(PathInfo.Create(HostingEnvironment.MapPath("~/")), PathInfo.Create(relativeGenerationPath)).ToString();
        }

        public ModelBuilderResult GenerateModels(bool overrideClasses)
        {
            var fileList = GetAllComponents();
            var schemaList = GetAllSchemas(fileList);

            return GenerateModels(schemaList, overrideClasses);
        }

        private ModelBuilderResult GenerateModels(IEnumerable<SchemaModel> schemaList, bool overrideClasses = false)
        {
            var timer = new Stopwatch();
            timer.Start();
            var oneFileText = new StringBuilder();
            var items = new List<ModelBuilderItem>();
            foreach (var schema in schemaList)
            {
                var generator = new CSharpGenerator(schema.Schema);
                generator.Settings.Namespace = schema.Namespace;
                generator.Settings.TypeNameGenerator = new NitroNetTypeNameGenerator();
                generator.Settings.ClassStyle = CSharpClassStyle.Poco;
                generator.Settings.GenerateDataAnnotations = schema.ShouldGenerateDataAnnotation;
                generator.Settings.HandleReferences = true;

                var dataFileString = generator.GenerateFile($"{schema.ClassName}Model");

                if (!Directory.Exists(_absoluteGenerationPath))
                {
                    Directory.CreateDirectory(_absoluteGenerationPath);
                }

                if (!_generateSingleFile)
                {
                    var item = CreateModelFile(dataFileString, schema, overrideClasses);
                    
                    if(item == null)
                    {
                        continue;
                    }

                    items.Add(item);
                }
                else
                {
                    oneFileText.Append("\n\n");
                    oneFileText.Append(dataFileString);
                }
            }

            if (_generateSingleFile)
            {
                var filePath = $"{_absoluteGenerationPath}\\NitroNetModels.cs";
                File.WriteAllText(filePath, oneFileText.ToString());
                var item = new ModelBuilderItem
                {
                    Name = filePath,
                    Size = oneFileText.Length
                };
                items.Add(item);
            }
            timer.Stop();
            var result = new ModelBuilderResult(items) { GenerationTime = timer.Elapsed };

            return result;
        }

        private ModelBuilderItem CreateModelFile(string dataFileString, SchemaModel schema, bool overrideClasses)
        {
            var folderStructure = schema.Namespace.Replace(_modelNamespace, "").Trim('.').Replace(".", "\\");
            var patternFolder = !string.IsNullOrEmpty(folderStructure) ? $"{_absoluteGenerationPath}\\{folderStructure}" : _absoluteGenerationPath;

            if (!Directory.Exists(patternFolder))
            {
                Directory.CreateDirectory(patternFolder);
            }

            var filePath = $"{patternFolder}\\{schema.ClassName}Model.cs";

            if (File.Exists(filePath) && overrideClasses != true)
            {
                return null;
            }

            File.WriteAllText(filePath, dataFileString);
            var item = new ModelBuilderItem
            {
                Name = filePath,
                Size = dataFileString.Length
            };

            return item;
        }

        private IEnumerable<SchemaModel> GetAllSchemas(IEnumerable<FileInfo> fileList)
        {
            var schemaList = new List<SchemaModel>();

            foreach (var jsonFile in fileList)
            {
                var model = new SchemaModel
                {
                    ClassName = jsonFile.Name.Replace(jsonFile.Extension, string.Empty),
                    Namespace = GenerateNamespace(jsonFile)
                };

                var jsonData = File.ReadAllText(jsonFile.FullName);
                JsonSchema4 schema;

                var generateDataAnnotations = false;

                if (jsonFile.Name.Equals("schema.json", StringComparison.InvariantCultureIgnoreCase))
                {
                    schema = JsonSchema4.FromJsonAsync(jsonData, jsonFile.Directory?.FullName).Result;
                    model.ClassName = $"Base{FileSystemUtility.FirstLetterToUpper(jsonFile.Directory?.Name)}";
                    generateDataAnnotations = true;
                }
                else
                {
                    schema = JsonSchema4.FromData(jsonData);
                }

                model.Schema = schema;
                schema.AllowAdditionalItems = true;
                schema.AllowAdditionalProperties = true;
                model.ShouldGenerateDataAnnotation = generateDataAnnotations;
                schemaList.Add(model);
            }

            return schemaList;
        }

        private string GenerateNamespace(FileSystemInfo jsonFile)
        {
            if (jsonFile == null)
            {
                return _modelNamespace;
            }

            var classNamespace = jsonFile.FullName.Replace(jsonFile.Name, jsonFile.Name.Replace(jsonFile.Extension, string.Empty)).Replace(_basePath.Replace("/", "\\"), string.Empty).Replace("/", ".").Replace("\\", ".").Replace("_data", string.Empty).Replace("..", ".");
            return PrettifyNamespace($"{_modelNamespace.TrimEnd('.')}.{classNamespace.TrimStart('.')}".TrimEnd('.').Replace("-", "_"));
        }

        private static string PrettifyNamespace(string baseNamespace)
        {
            if (string.IsNullOrEmpty(baseNamespace))
            {
                return baseNamespace;
            }

            var prettyNamespaceSplitted = baseNamespace.Split('.');
            var prettyNamespace = new StringBuilder();
            foreach (var segment in prettyNamespaceSplitted)
            {
                prettyNamespace.Append(char.IsDigit(segment[0]) ? $"_{segment}" : $"{FileSystemUtility.FirstLetterToUpper(segment)}.");
            }

            return prettyNamespace.ToString().TrimEnd('.');
        }

        private IEnumerable<FileInfo> GetAllComponents()
        {
            var fileList = new List<FileInfo>();

            try
            {
                foreach (var componentPath in _nitronetConfig.ComponentPaths)
                {
                    var fullpath = $"{_basePath.TrimEnd('/')}/{componentPath}";

                    fileList.AddRange(FileSystemUtility.DirSearch(fullpath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return fileList;
        }
    }
}
