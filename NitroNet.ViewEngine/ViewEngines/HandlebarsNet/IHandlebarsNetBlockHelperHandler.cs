using HandlebarsDotNet;
using System.IO;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public interface IHandlebarsNetBlockHelperHandler
    {
        void Evaluate(TextWriter output, HelperOptions options, dynamic context, object[] parameters);
    }
}
