using System.IO;

namespace NitroNet.HandlebarsNet.ViewEngine
{
    public interface IHandlebarsNetHelperHandler
    {
        void Evaluate(TextWriter output, dynamic context, params object[] parameters);
    }
}
