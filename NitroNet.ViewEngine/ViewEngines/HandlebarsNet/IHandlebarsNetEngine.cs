using System;
using System.IO;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public interface IHandlebarsNetEngine
    {
        Func<object, string> Compile(string content);
        Action<TextWriter, object> Compile(TextReader textReader);
        Func<object, string> CompileView(string templatePath);
    }
}
