using System;
using System.Collections.Generic;
using NitroNet.ViewEngine.Context;

namespace NitroNet.ViewEngine.TemplateHandler.Grid
{
	public class GridStack
	{
		private const string Gridstackkey = "gridstack";
		private readonly Stack<GridContext> _contexts = new Stack<GridContext>();

		private GridStack()
		{
			_contexts.Push(new GridContext(null, 0));
		}

		public void Push(double width, double? componentPadding = null)
		{
			_contexts.Push(new GridContext(Current, width, componentPadding));
		}

		public GridContext Current
		{
			get
			{
				if (_contexts.Count <= 0)
					throw new InvalidOperationException("No grid context found.");

				return _contexts.Peek();
			}
		}

		public void Pop()
		{
			_contexts.Pop();
		}

		public static GridStack FromContext(RenderingContext renderingContext)
		{
			object stack;
			GridStack gridStack;
			if (!renderingContext.Data.TryGetValue(Gridstackkey, out stack))
			{
				gridStack = new GridStack();
				renderingContext.Data.Add(Gridstackkey, gridStack);
			}
			else
			{
				gridStack = stack as GridStack;
				if (gridStack == null)
				{
					gridStack = new GridStack();
					renderingContext.Data[Gridstackkey] = gridStack;
				}
			}
			return gridStack;
		}
	}
}