using NitroNet.ModelBuilder.Generator.Models;

namespace NitroNet.ModelBuilder.Generator
{
    public interface IModelBuilder
    {
        ModelBuilderResult GenerateModels(bool overrideClasses);
        bool GenerateSingleFile { get; }
    }
}
