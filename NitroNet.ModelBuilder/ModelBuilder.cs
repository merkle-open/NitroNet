using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Hosting;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace NitroNet.ModelBuilder
{
    public class ModelBuilder : IModelBuilder
    {
        private readonly INitroNetConfig _nitronetConfig;
        private readonly string _basePath;
        private readonly string _modelNamespace;
        private readonly bool _generateSingleFile;
        private readonly string _absoluteGenerationPath;

        public ModelBuilder(string basePath, INitroNetConfig configuration)
        {
            var relativeGenerationPath = ConfigurationManager.AppSettings["NitroNet.Generation.Path"] ?? "Models";
            var generateSingleFileString = ConfigurationManager.AppSettings["NitroNet.Generation.SingleFile"] ?? "true";

            _nitronetConfig = configuration;
            _basePath = basePath;
            _modelNamespace = ConfigurationManager.AppSettings["NitroNet.ModelNamespace"] ?? "NitroNet.Models";
            _generateSingleFile = !generateSingleFileString.Equals("false", StringComparison.InvariantCultureIgnoreCase);
            _absoluteGenerationPath = PathInfo.Combine(PathInfo.Create(HostingEnvironment.MapPath("~/")), PathInfo.Create(relativeGenerationPath)).ToString();
        }

        public void GenerateModels()
        {
            var fileList = GetAllFiles();
            var schemaList = GetAllSchemas(fileList);

            GenerateModels(schemaList);
        }

        private void GenerateModels(IEnumerable<SchemaModel> schemaList)
        {
            var oneFileText = new StringBuilder();

            foreach (var schema in schemaList)
            {
                // TODO: Override the TypeNameGenerator? I dont like anonymous classes...
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
                    File.WriteAllText($"{_absoluteGenerationPath}\\{schema.ClassName}Model.cs", dataFileString);
                }
                else
                {
                    oneFileText.Append("\n\n");
                    oneFileText.Append(dataFileString);
                }
            }

            if (_generateSingleFile)
            {
                File.WriteAllText($"{_absoluteGenerationPath}\\NitroNetModels.cs", oneFileText.ToString());
            }
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
                    schema = JsonSchema4.FromJsonAsync(jsonData, jsonFile.Directory.FullName).Result;
                    model.ClassName = $"Base{FirstLetterToUpper(jsonFile.Directory.Name)}";
                    generateDataAnnotations = true;
                }
                else
                {
                    // TODO: Add definitions and References to the Schema
                    schema = JsonSchema4.FromData(jsonData);
                }

                model.Schema = schema;
                schema.AllowAdditionalItems = true;
                schema.AllowAdditionalProperties = true;
                model.ShouldGenerateDataAnnotation = generateDataAnnotations;
                schemaList.Add(model);
            }

            foreach (var schemaModel in schemaList)
            {
                foreach (var schemaProperty in schemaModel.Schema.Properties)
                {
                    if(schemaProperty.Value.IsEnumeration || schemaProperty.Value.IsDictionary || schemaProperty.Value.Type == JsonObjectType.Object || schemaProperty.Value.Type == JsonObjectType.Array)
                    {
                        schemaProperty.Value.Title = schemaProperty.Value.Name;
                    }
                }
            }

            return schemaList;
        }

        private string GenerateNamespace(FileSystemInfo jsonFile)
        {
            if(jsonFile == null)
            {
                return _modelNamespace;
            }

            var classNamespace = jsonFile.FullName.Replace(jsonFile.Name, jsonFile.Name.Replace(jsonFile.Extension, string.Empty)).Replace(_basePath.Replace("/", "\\"), string.Empty).Replace("/", ".").Replace("\\", ".").Replace("_data", string.Empty).Replace("..", ".");
            return PrettifyNamespace($"{_modelNamespace.TrimEnd('.')}.{classNamespace.TrimStart('.')}".TrimEnd('.').Replace("-", "_"));
        }

        private string PrettifyNamespace(string baseNamespace)
        {
            if(string.IsNullOrEmpty(baseNamespace))
            {
                return baseNamespace;
            }

            var prettyNamespaceSplitted = baseNamespace.Split('.');
            var prettyNamespace = new StringBuilder();
            foreach (var segment in prettyNamespaceSplitted)
            {
                prettyNamespace.Append(char.IsDigit(segment[0]) ? $"_{segment}" : $"{FirstLetterToUpper(segment)}.");
            }

            return prettyNamespace.ToString().TrimEnd('.');
        }

        private List<FileInfo> GetAllFiles()
        {
            var fileList = new List<FileInfo>();

            try
            {
                foreach (var componentPath in _nitronetConfig.ComponentPaths)
                {
                    var fullpath = $"{_basePath.TrimEnd('/')}/{componentPath}";

                    fileList.AddRange(DirSearch(fullpath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return fileList;
        }

        private string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        private IEnumerable<FileInfo> DirSearch(string sDir)
        {
            try
            {
                var fileList = new List<FileInfo>();
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d, "*.json"))
                    {
                        fileList.Add(new FileInfo(f));
                    }
                    fileList.AddRange(DirSearch(d));
                }

                return fileList;
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
                return new List<FileInfo>();
            }
        }
    }
}
