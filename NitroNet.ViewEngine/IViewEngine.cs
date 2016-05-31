using System;
using System.Threading.Tasks;

namespace NitroNet.ViewEngine
{
    public interface IViewEngine
    {
        Task<IView> CreateViewAsync(TemplateInfo templateInfo, Type modelType);
    }

    public static class ViewEngineExtension
    {
        public static Task<IView> CreateViewAsync(this IViewEngine viewEngine, TemplateInfo templateInfo)
        {
            return viewEngine.CreateViewAsync(templateInfo, typeof (object));
        }
    }
}