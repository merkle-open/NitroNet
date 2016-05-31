using System;
using System.Threading.Tasks;

namespace NitroNet.ViewEngine
{
    public interface IModelTypeProvider
    {
        Task<Type> GetModelTypeFromTemplateAsync(TemplateInfo templateInfo);
    }
}
