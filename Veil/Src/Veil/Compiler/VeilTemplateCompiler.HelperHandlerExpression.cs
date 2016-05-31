using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Veil.Helper;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    public static class HelperWrapper
    {
        public static void Evaluate(IHelperHandler handler, object model, RenderingContext renderingContext, HelperExpressionNode node)
        {
			handler.Evaluate(model, renderingContext, node.Parameters);
        }

		public static void Leave(IBlockHelperHandler handler, object model, RenderingContext renderingContext, HelperExpressionNode node)
		{
			handler.Leave(model, renderingContext, node.Name, node.Parameters);
		}
    }

	class VeilTemplateCompiler
	{
		internal static readonly MethodInfo HelperFunction = typeof(HelperWrapper).GetMethod("Evaluate");
		internal static readonly MethodInfo HelperFunctionAsync = typeof(HelperWrapper).GetMethod("EvaluateAsync");
		internal static readonly MethodInfo HelperFunctionLeaveAsync = typeof(HelperWrapper).GetMethod("LeaveAsync");
		internal static readonly MethodInfo HelperFunctionLeave = typeof(HelperWrapper).GetMethod("Leave");
	}

	internal partial class VeilTemplateCompiler<T> : VeilTemplateCompiler
    {
		private Expression HandleHelperExpression(HelperExpressionNode node)
		{
			return HandleHelperExpressionWithMethod(node, HelperFunction);
		}

		private Expression HandleHelperExpressionAsync(HelperExpressionNode node)
		{
			return HandleHelperExpressionWithMethodAsync(node, HelperFunctionAsync);
		}

	    private Expression HandleHelperExpressionWithMethod(HelperExpressionNode node, MethodInfo helperFunction)
	    {
		    var helper = EvaluateHelper(node);
		    var modelExpression = EvaluateScope(node);

	        var expression = Expression.Call(helperFunction,
                Expression.Constant(helper),
	            modelExpression, _context, Expression.Constant(node));

	        return expression;
	    }

		private Expression HandleHelperExpressionWithMethodAsync(HelperExpressionNode node, MethodInfo helperFunction)
		{
			var helper = EvaluateHelper(node);
			var modelExpression = EvaluateScope(node);

			var expression = Expression.Call(helperFunction,
				_task,
				Expression.Constant(helper),
				modelExpression, _context, Expression.Constant(node));

			return expression;
		}

	    private IHelperHandler EvaluateHelper(HelperExpressionNode node)
		{
			var helper = _helperHandlers.FirstOrDefault(h => h.IsSupported(node.Name));
			if (helper == null)
				throw new VeilCompilerException(string.Format("Could not find a helper with the name '{0}'.", node.Name), node);

			return helper;
		}

	    private Expression HandleHelperBlockNode(HelperBlockNode node)
	    {
			return HandleBlock(
				HandleHelperExpression(node.HelperExpression), 
				HandleBlock(node.Block),
                HandleHelperExpressionWithMethod(node.HelperExpression, HelperFunctionLeave));
	    }

		private Expression HandleHelperBlockNodeAsync(HelperBlockNode node)
		{
			return HandleBlock(
				HandleHelperExpressionAsync(node.HelperExpression),
				HandleBlockAsync(node.Block),
				HandleHelperExpressionWithMethodAsync(node.HelperExpression, HelperFunctionLeaveAsync));
		}
    }
}