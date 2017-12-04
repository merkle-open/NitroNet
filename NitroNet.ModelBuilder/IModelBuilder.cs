namespace NitroNet.ModelBuilder
{
    public interface IModelBuilder
    {
        ModelBuilderResult GenerateModels(bool overrideClasses);
        bool GenerateSingleFile { get; }
    }
}
