using System.IO;
using System.Linq;
using HandlebarsDotNet.Compiler;
using NitroNet.HandlebarsNet.ViewEngine;

namespace NitroNet.HandlebarsNet.Handlers
{
    public class HandlebarsNetTemplateIdHandler : IHandlebarsNetHelperHandler
    {
        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            var parametersAsDictionary = (HashParameterDictionary)parameters.First();
            var templateId = parametersAsDictionary["templateId"];
            output.Write(templateId as string);
        }
    }
}
