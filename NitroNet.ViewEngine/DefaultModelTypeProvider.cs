using System;
using System.Threading.Tasks;

namespace NitroNet.ViewEngine
{
    public class DefaultModelTypeProvider : IModelTypeProvider
    {
        public Task<Type> GetModelTypeFromTemplateAsync(TemplateInfo templateInfo)
        {
            return Task.FromResult(typeof(object));
        }
    }
}
