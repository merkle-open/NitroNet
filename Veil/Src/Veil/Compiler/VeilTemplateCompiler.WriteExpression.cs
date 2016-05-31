using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
	internal partial class VeilTemplateCompiler<T>
	{
		private static readonly MethodInfo writeMethodAsync = typeof(Helpers).GetMethod("WriteAsync", new[] { typeof(Task), typeof(TextWriter), typeof(string) });
		private static readonly MethodInfo writeMethod = typeof(Helpers).GetMethod("Write", new[] { typeof(TextWriter), typeof(string) });
		//private static readonly MethodInfo writeMethodObject = typeof(Helpers).GetMethod("WriteAsync", new[] { typeof(object) });
        private static readonly MethodInfo writeMethodObject = typeof(Helpers).GetMethod("Write", new[] { typeof(TextWriter), typeof(object) });
        private static readonly MethodInfo writeMethodObjectAsync = typeof(Helpers).GetMethod("WriteAsync", new[] { typeof(Task), typeof(TextWriter), typeof(object) });
		private static readonly MethodInfo encodeMethodAsync = typeof(Helpers).GetMethod("HtmlEncodeAsync", new[] { typeof(Task), typeof(TextWriter), typeof(string) });
		private static readonly MethodInfo encodeMethod = typeof(Helpers).GetMethod("HtmlEncode", new[] { typeof(TextWriter), typeof(string) });
		private static readonly MethodInfo encodeMethodObjectAsync = typeof(Helpers).GetMethod("HtmlEncodeAsync", new[] { typeof(Task), typeof(TextWriter), typeof(object) });
		private static readonly MethodInfo encodeMethodObject = typeof(Helpers).GetMethod("HtmlEncode", new[] { typeof(TextWriter), typeof(object) });
		//private static readonly MethodInfo chainMethod = typeof(TaskHelper).GetMethod("ChainTask", new[] { typeof(Task), typeof(Func<Task>) });

		private Expression HandleWriteExpressionAsync(WriteExpressionNode node)
		{
			bool escapeHtml;
			var expression = ParseExpression(node.Expression, out escapeHtml);

			if (node.HtmlEncode && escapeHtml)
			{
				if (expression.Type == typeof(string))
					return Expression.Call(encodeMethodAsync, _task, _writer, expression);

                return Expression.Call(encodeMethodObjectAsync, _task, _writer, Expression.Convert(expression, typeof(object)));
			}

			if (expression.Type == typeof(string))
				return Expression.Call(writeMethodAsync, _task, _writer, expression);

			if (expression.Type == typeof(void))
				return expression;

            return Expression.Call(encodeMethodObjectAsync, _writer, Expression.Convert(expression, typeof(object)));
		}

		private Expression HandleWriteExpression(WriteExpressionNode node)
		{
			bool escapeHtml;
			var expression = ParseExpression(node.Expression, out escapeHtml);

			if (node.HtmlEncode && escapeHtml)
			{
				if (expression.Type == typeof(string))
					return Expression.Call(encodeMethod, _writer, expression);

                return Expression.Call(encodeMethodObject, _writer, Expression.Convert(expression, typeof(object)));
			}

			if (expression.Type == typeof(string))
				return Expression.Call(writeMethod, _writer, expression);

			if (expression.Type == typeof(void))
				return expression;

            return Expression.Call(writeMethodObject, _writer, Expression.Convert(expression, typeof(object)));
		}
	}
}