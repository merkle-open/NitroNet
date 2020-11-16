using System;
using System.Collections.Generic;
using NitroNet.ViewEngine.Context;
using NitroNet.ViewEngine.TemplateHandler.Models;

namespace NitroNet.ViewEngine.TemplateHandler.Utils
{
    public interface INitroTemplateHandlerUtils
    {
        SubModel FindSubModel(IDictionary<string, RenderingParameter> renderingParameters, object model, RenderingContext context);

        IDictionary<string, ResolvedAdditionalArgument> ResolveAdditionalArguments(object model, IDictionary<string, string> parameters, ISet<string> reservedKeys);

        IDictionary<string, ResolvedAdditionalArgument> ConvertAdditionalArguments(IDictionary<string, object> parameters, ISet<string> reservedKeys);

        bool TryCreateModel(SubModel submodel, IDictionary<string, ResolvedAdditionalArgument> additionalArguments,
            out object model);

        void ApplyResolvedArgumentsToObject(object target,
            IDictionary<string, ResolvedAdditionalArgument> additionalArguments);

        void RenderPartial(object subModel, string componentValue, string skinValue, RenderingContext renderingContext,
            Action<string, object, RenderingContext> renderPartial);

        void ThrowErrorIfSubModelFoundAndNull(bool modelFound, object subModel, string propertyName, object model);

        string CleanName(string text);

        bool GetPropertyValueFromObjectHierarchically(object model, string propertyName, out object modelValue);

        bool IsValid(SubModel subModel);
    }
}