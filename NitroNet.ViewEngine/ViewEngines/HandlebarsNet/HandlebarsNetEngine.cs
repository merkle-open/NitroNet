using HandlebarsDotNet;
using System;
using System.IO;
using System.Linq;

namespace NitroNet.ViewEngine.ViewEngines.HandlebarsNet
{
    public class HandlebarsNetEngine : IHandlebarsNetEngine
    {
        private readonly IHandlebarsNetHelperHandlerFactory _helperHandlerFactory;

        public HandlebarsNetEngine(IHandlebarsNetHelperHandlerFactory helperHandlerFactory)
        {
            _helperHandlerFactory = helperHandlerFactory;

            var helpers = _helperHandlerFactory.Create();
            Handlebars.RegisterHelper("pattern", helpers.First().Evaluate);

            Handlebars.RegisterHelper("partial", (output, context, arguments) =>
            {
                var partialName = arguments[0].ToString().Trim('"', '\'');
                var renderer = Handlebars.Compile(partialName);

                output.Write(renderer(context));
            });

            Handlebars.RegisterHelper("placeholder", (output, context, arguments) =>
            {
                output.Write(context);
            });

            Handlebars.RegisterHelper("template-id", (output, context, arguments) =>
            {
                output.Write(context);
            });

            Handlebars.RegisterHelper("grid-cell", (output, context, arguments) =>
            {
                output.Write(context);
            });

            Handlebars.RegisterHelper("grid-width", (output, context, arguments) =>
            {
                output.Write(context);
            });

            Handlebars.RegisterHelper("l", (output, context, arguments) =>
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
