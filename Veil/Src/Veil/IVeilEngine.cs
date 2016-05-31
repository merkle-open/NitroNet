using System;
using System.IO;
using System.Threading.Tasks;
using Veil.Parser;

namespace Veil
{
    /// <summary>
    /// An interface for <see cref="VeilEngine"/> provided for testers
    /// </summary>
    public interface IVeilEngine
    {
	    /// <summary>
	    /// Parses and compiles the specified template
	    /// </summary>
	    /// <typeparam name="T">The type of the model that will be passed to the template</typeparam>
	    /// <param name="templateId"></param>
	    /// <param name="parserKey">Key of the <see cref="Veil.Parser.ITemplateParser"/> to use to parse the template.</param>
	    /// <param name="templateContents">The contents of the template to compile</param>
	    /// <returns>A compiled action ready to be executed as needed to render the template</returns>
		Action<RenderingContext, T> Compile<T>(string templateId, string parserKey, TextReader templateContents);
		Action<RenderingContext, T> Compile<T>(string templateId, ITemplateParser parser, TextReader templateContents);
		
	    /// <summary>
	    /// Parses and compiles the specified template when the model type is not known
	    /// </summary>
	    /// <param name="templateId"></param>
	    /// <param name="parserKey">Key of the <see cref="Veil.Parser.ITemplateParser"/> to use to parse the template.</param>
	    /// <param name="templateContents">The contents of the template to compile</param>
	    /// <param name="modelType">The type of the model that will be passed to the template</param>
	    /// <returns>A compiled action that will cast the model before execution</returns>
		Action<RenderingContext, object> CompileNonGeneric(string templateId, string parserKey, TextReader templateContents, Type modelType);
		Action<RenderingContext, object> CompileNonGeneric(string templateId, ITemplateParser parser, TextReader templateContents, Type modelType);
		
        Action<RenderingContext, T> CompileFromTree<T>(SyntaxTreeNode syntaxTree);
    }
}