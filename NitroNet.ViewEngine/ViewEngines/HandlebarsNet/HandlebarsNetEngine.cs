using HandlebarsDotNet;
using System;
using System.IO;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public class HandlebarsNetEngine : IHandlebarsNetEngine
    {
        private readonly IHandlebarsNetHelperHandlerFactory _helperHandlerFactory;

        public HandlebarsNetEngine(IHandlebarsNetHelperHandlerFactory helperHandlerFactory)
        {
            Handlebars.Configuration.TextEncoder = null;
            _helperHandlerFactory = helperHandlerFactory;
            
            var helpers = _helperHandlerFactory.Create();
            helpers.ForEach(h => Handlebars.Configuration.Helpers.Add(h));
            
            Handlebars.RegisterHelper("grid-cell", (output, context, arguments) =>
            {
                output.Write(context);
            });

            Handlebars.RegisterHelper("grid-width", (output, context, arguments) =>
            {
                output.Write(context);
            });
        }

        public Func<object, string> Compile(string content)
        {
            return Handlebars.Compile(content);
        }

        public Action<TextWriter, object> Compile(TextReader textReader)
        {
            return Handlebars.Compile(textReader);
        } 

        public Func<object, string> CompileView(string templatePath)
        {
            return Handlebars.CompileView(templatePath);
        }
    }
}
