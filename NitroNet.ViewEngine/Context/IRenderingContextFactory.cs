using System.IO;

namespace NitroNet.ViewEngine.Context
{
    public interface IRenderingContextFactory
    {
        RenderingContext Create(TextWriter writer, object model);

        RenderingContext GetOrCreate(TextWriter writer, object model);
    }
}
