using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet.Grid
{
    internal class HandlebarsNetGridStack
    {
        private const string Gridstackkey = "gridstack";
        private readonly Stack<HandlebarsNetGridContext> _contexts = new Stack<HandlebarsNetGridContext>();

        private HandlebarsNetGridStack()
        {
            _contexts.Push(new HandlebarsNetGridContext(null, 0));
        }

        public void Push(double width, double? componentPadding = null)
        {
            _contexts.Push(new HandlebarsNetGridContext(Current, width, componentPadding));
        }

        public HandlebarsNetGridContext Current
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

        public static HandlebarsNetGridStack FromContext(ViewContext viewContext)
        {
            object stack;
            HandlebarsNetGridStack gridStack;
            if (!viewContext.ViewData.TryGetValue(Gridstackkey, out stack))
            {
                gridStack = new HandlebarsNetGridStack();
                viewContext.ViewData.Add(Gridstackkey, gridStack);
            }
            else
            {
                gridStack = stack as HandlebarsNetGridStack;
                if (gridStack == null)
                {
                    gridStack = new HandlebarsNetGridStack();
                    viewContext.ViewData[Gridstackkey] = gridStack;
                }
            }
            return gridStack;
        }
    }
}
