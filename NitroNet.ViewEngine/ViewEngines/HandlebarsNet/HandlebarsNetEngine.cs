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
            _helperHandlerFactory = helperHandlerFactory;
            Handlebars.Configuration.TextEncoder = null;
            
            var helpers = _helperHandlerFactory.Create();
            helpers.ForEach(h => Handlebars.Configuration.Helpers.Add(h));

            var blockHelpers = _helperHandlerFactory.CreateForBlocks();
            blockHelpers.ForEach(b => Handlebars.Configuration.BlockHelpers.Add(b));
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
