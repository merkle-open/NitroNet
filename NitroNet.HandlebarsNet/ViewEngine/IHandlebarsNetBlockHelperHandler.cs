using System.IO;
using HandlebarsDotNet;

namespace NitroNet.HandlebarsNet.ViewEngine
{
    public interface IHandlebarsNetBlockHelperHandler
    {
        void Evaluate(TextWriter output, HelperOptions options, dynamic context, object[] parameters);
    }
}
