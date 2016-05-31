using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private readonly ParameterExpression _task = Expression.Variable(typeof(Task), "task");

        private Expression HandleWriteLiteralAsync(WriteLiteralNode node)
        {
            var callExpression = Expression.Call(writeMethodAsync, _task, this._writer, Expression.Constant(node.LiteralContent, typeof(string)));
            return callExpression;
        }

		private Expression HandleWriteLiteral(WriteLiteralNode node)
		{
			var callExpression = Expression.Call(writeMethod, this._writer, Expression.Constant(node.LiteralContent, typeof(string)));
			return callExpression;
		}

        private Expression HandleAsync(Expression callExpression)
        {
            if (callExpression.Type == typeof(Task))
            {
                if (callExpression.NodeType == ExpressionType.Block)
                {
                    return callExpression;
                }

                return Expression.Assign(_task, callExpression);
                //return Expression.Assign(_task, Expression.Call(chainMethod, new[] { _task, callExpression }));
            }

            return callExpression;
            //var expr = Expression.Lambda<Action>(callExpression);
            //return Expression.Assign(_task, Expression.Call(chainMethod, _task, expr));
        }
    }
}