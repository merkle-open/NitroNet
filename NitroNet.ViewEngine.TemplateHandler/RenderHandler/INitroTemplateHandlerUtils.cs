using System;
using System.Collections.Generic;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler.RenderHandler
{
    public interface INitroTemplateHandlerUtils
    {
        SubModel FindSubModel(RenderingParameter component, RenderingParameter skin,
            RenderingParameter dataVariation, object model, RenderingContext context);
        IDictionary<string, ResolvedParameter> ResolveAdditionalParameters(object model, IDictionary<string, string> parameters);
        void ApplyResolvedParameters(object target, IDictionary<string, ResolvedParameter> parameters);
        void RenderPartial(object subModel, string componentValue, string skinValue,
            RenderingContext renderingContext, Action<string, object, RenderingContext> renderPartial);
        void ThrowErrorIfSubModelFoundAndNull(bool modelFound, object subModel, string propertyName, object model);
        string CleanName(string text);
        bool GetPropertyValueFromObjectHierarchically(object model, string propertyName, out object modelValue);
    }
}
