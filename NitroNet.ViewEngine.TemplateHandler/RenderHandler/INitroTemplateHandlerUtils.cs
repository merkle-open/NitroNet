using Veil;

namespace NitroNet.ViewEngine.TemplateHandler.RenderHandler
{
    public interface INitroTemplateHandlerUtils
    {
        PropertyAssignments DoPropertyAssignments(RenderingParameter component, RenderingParameter skin, RenderingParameter dataVariation, object model, RenderingContext context);
        string GetComponentId(string componentId, string skin);
        string CleanName(string text);
        bool GetValueFromObjectHierarchically(object model, string propertyName, out object modelValue);
        void ThrowExceptionIfPropertyNull(bool modelFound, object subModel, string propertyName, object model);
    }
}
