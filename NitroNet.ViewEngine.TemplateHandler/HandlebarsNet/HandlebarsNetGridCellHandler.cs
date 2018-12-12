using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;
using System.Collections.Generic;
using System.IO;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet
{
    public class HandlebarsNetGridCellHandler : IHandlebarsNetHelperHandler
    {
        private static readonly Dictionary<string, double> DefaultRatioTable = new Dictionary<string, double>
        {
            {"1/4", 0.25},
            {"1/2", 0.5},
            {"3/4", 0.75},
            {"1/3", (double)1/3},
            {"2/3", (double)2/3},
            {"1/5", (double)1/5}
        };

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
