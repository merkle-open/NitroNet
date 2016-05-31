using System.Collections.Generic;
using System.Threading.Tasks;

namespace NitroNet.ViewEngine
{
    public interface IComponentRepository
    {
        IEnumerable<ComponentDefinition> GetAll();

        Task<ComponentDefinition> GetComponentDefinitionByIdAsync(string id);
    }
}
