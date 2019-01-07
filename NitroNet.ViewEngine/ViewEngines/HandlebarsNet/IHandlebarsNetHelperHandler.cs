using System.IO;
using HandlebarsDotNet;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public interface IHandlebarsNetHelperHandler
    {
        void Evaluate(TextWriter output, dynamic context, params object[] parameters);
    }
}
