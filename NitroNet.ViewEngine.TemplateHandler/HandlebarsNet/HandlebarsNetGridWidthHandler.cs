using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet
{
    internal abstract class BaseHandlebarsNetGridWidthHandler : IHandlebarsNetHelperHandler
    {
        public abstract bool IsSupported(string name);

        internal abstract double GetWidth(GridStack gridStack);

        public void Evaluate(TextWriter output, dynamic context, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
