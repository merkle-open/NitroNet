using System;
using System.IO;
using HandlebarsDotNet;

namespace NitroNet.HandlebarsNet.ViewEngine
{
    public class HandlebarsNetEngine : IHandlebarsNetEngine
    {
        private readonly IHandlebarsNetHelperHandlerFactory _helperHandlerFactory;

        public HandlebarsNetEngine(IHandlebarsNetHelperHandlerFactory helperHandlerFactory)
        {
            _helperHandlerFactory = helperHandlerFactory;
            Handlebars.Configuration.TextEncoder = null;

            var helpers = _helperHandlerFactory.Create();

            foreach (var helper in helpers)
            {
                Handlebars.RegisterHelper(helper.Key, helper.Value);
            }

            var blockHelpers = _helperHandlerFactory.CreateForBlocks();

            foreach (var blockHelper in blockHelpers)
            {
                Handlebars.RegisterHelper(blockHelper.Key, blockHelper.Value);
            }
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
