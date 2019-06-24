using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine.Config
{
    public static class ConfigurationLoader
    {
        public static INitroNetConfig LoadNitroConfiguration(string basePath)
        {
            return LoadNitroConfiguration(basePath, "nitronet-config.json");
        }

        public static INitroNetConfig LoadNitroConfiguration(string basePath, string fileName)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            var basePathInfo = PathInfo.Create(basePath);
            var configFile = PathInfo.Combine(basePathInfo, PathInfo.Create(fileName));

            if (!File.Exists(configFile.ToString()))
            {
                throw new ConfigurationException($"Could not find configuration in path '{configFile}' in {basePathInfo}.");
            }

            NitroNetJsonConfig jsonConfig;
            using (var reader = new JsonTextReader(new StreamReader(new FileStream(configFile.ToString(), FileMode.Open, FileAccess.Read))))
            {
                jsonConfig = new JsonSerializer().Deserialize<NitroNetJsonConfig>(reader);
            }

            return new NitroNetConfig
            {
                ViewPaths =
                    jsonConfig.ViewPaths.Select(path => GetDefaultValueIfNotSet(path, PathInfo.Create("views"))),
                Extensions = jsonConfig.Extensions.Select(extension => GetDefaultValueIfNotSet(extension, "html")),
                Filters = jsonConfig.Filters.Select(filter => GetDefaultValueIfNotSet(filter, string.Empty)),
                PartialPaths =
                    jsonConfig.PartialPaths.Select(path => GetDefaultValueIfNotSet(path, PathInfo.Create("partials"))),
                ComponentPaths = jsonConfig.ComponentPaths.Select(path => GetDefaultValueIfNotSet(path,
                    PathInfo.Create("components"),
                    PathInfo.Create("modules"))),
                AdditionalArgumentsOnlyComponents = jsonConfig.AdditionalArgumentsOnlyComponents
                AdditionalArgumentsParsingMode = jsonConfig.AdditionalArgumentsParsingMode,
            };
        }

        private static string GetDefaultValueIfNotSet(string extension, string defaultValue)
        {
            return string.IsNullOrEmpty(extension) ? defaultValue : extension;
        }


        private static PathInfo GetDefaultValueIfNotSet(string value, params PathInfo[] defaultLocation)
        {
            if (string.IsNullOrEmpty(value))
            {
                return PathInfo.Combine(defaultLocation);
            }

            return PathInfo.Create(value);
        }
    }
}