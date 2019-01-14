using System.IO;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public interface IHandlebarsNetHelperHandler
    {
        void Evaluate(TextWriter output, dynamic context, params object[] parameters);
    }
}
