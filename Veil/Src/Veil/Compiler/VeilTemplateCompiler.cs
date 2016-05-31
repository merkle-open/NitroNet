using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Veil.Helper;
using Veil.Parser;

namespace Veil.Compiler
{
	internal partial class VeilTemplateCompiler<T>
	{
		private readonly ParameterExpression _context = Expression.Parameter(typeof(RenderingContext), "context");
		private readonly Expression _writer;

		private readonly ParameterExpression _model = Expression.Parameter(typeof(T), "model");
        private readonly LinkedList<Expression> _modelStack = new LinkedList<Expression>();
		private readonly IDictionary<string, SyntaxTreeNode> _overrideSections = new Dictionary<string, SyntaxTreeNode>();
		private readonly IHelperHandler[] _helperHandlers;

		public VeilTemplateCompiler(params IHelperHandler[] helperHandlers)
		{
			_helperHandlers = helperHandlers;
			this._writer = Expression.Property(_context, "Writer");
		}

		public Func<RenderingContext, T, Task> CompileAsync(SyntaxTreeNode templateSyntaxTree)
		{
			this.PushScope(this._model);

			var bodyExpression = this.HandleNodeAsync(templateSyntaxTree);

		    var innerExpression = Expression.Block(typeof(Task), new[] {_task}, 
                Expression.Assign(_task, Expression.Constant(Task.FromResult(false))), 
                bodyExpression,
                _task);

            var expression = Expression.Lambda<Func<RenderingContext, T, Task>>(innerExpression, this._context, this._model);

			return expression.Compile(); 
		}

		public Action<RenderingContext, T> Compile(SyntaxTreeNode templateSyntaxTree)
		{
			this.PushScope(this._model);

			var bodyExpression = this.HandleNode(templateSyntaxTree);

			var expression = Expression.Lambda<Action<RenderingContext, T>>(bodyExpression, this._context, this._model);

			return expression.Compile();
		}

        private void PushScope(Expression scope)
		{
			this._modelStack.AddFirst(scope);
		}

		private void PopScope()
		{
			this._modelStack.RemoveFirst();
		}
	}
}