using Microsoft.Practices.Unity;
using NitroNet.ViewEngine.Config;

namespace NitroNet.UnityModules
{
    public class NitroNetApplication
    {
        public string Name { get; private set; }
        public string Section { get; private set; }
        private INitroNetConfig Configuration { get; set; }
        public IUnityContainer Container { get; private set; }

        public NitroNetApplication(string name, string section, INitroNetConfig configuration, IUnityContainer container)
        {
            Name = name;
            Section = section;
            Configuration = configuration;
            Container = container;
        }
    }
}