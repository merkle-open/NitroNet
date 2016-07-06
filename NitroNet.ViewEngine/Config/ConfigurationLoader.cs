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
            return LoadNitroConfiguration(basePath, "config.json");
        }

        public static INitroNetConfig LoadNitroConfiguration(string basePath, string fileName)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException("basePath");
            }

            var basePathInfo = PathInfo.Create(basePath);
            var configFile = PathInfo.Combine(basePathInfo, PathInfo.Create(fileName));

            if (!File.Exists(configFile.ToString()))
                throw new ConfigurationException(string.Format("Could not find configuration in path '{0}' in {1}.", configFile, basePathInfo));

            NitroNetJsonConfig jsonConfig;
            using (var reader = new JsonTextReader(new StreamReader(new FileStream(configFile.ToString(), FileMode.Open, FileAccess.Read))))
            {
                jsonConfig = new JsonSerializer().Deserialize<NitroNetJsonConfig>(reader);
            }

            return new NitroNetConfig
            {
                ViewPaths = jsonConfig.ViewPaths.Select(path => GetDefaultValueIfNotSet(path, PathInfo.Create("views"))),
                PartialPaths = jsonConfig.PartialPaths.Select(path => GetDefaultValueIfNotSet(path, PathInfo.Create("partials"))),
                ComponentPaths = jsonConfig.ComponentPaths.Select(path => GetDefaultValueIfNotSet(path, PathInfo.Create("components"),
                    PathInfo.Create("modules")))
            };
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