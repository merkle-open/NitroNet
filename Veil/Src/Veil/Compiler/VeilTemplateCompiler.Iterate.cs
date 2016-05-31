using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private static MethodInfo moveNextMethod = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
        private static MethodInfo disposeMethod = typeof(IDisposable).GetMethod("Dispose");
        private static MethodInfo checkNotNullMethod = typeof(Helpers).GetMethod("CheckNotNull");
        private static MethodInfo checkNullMethod = typeof(Helpers).GetMethod("CheckNull");

        private Expression HandleIterateAsync(IterateNode node)
        {
            if (node.Collection.ResultType.IsArray)
            {
                return HandleIterateArray(node);
            }

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(node.ItemType);
            var getEnumeratorMethod = enumerableType.GetMethod("GetEnumerator");
            var getCurrentMethod = getEnumeratorMethod.ReturnType.GetProperty("Current").GetGetMethod();

            var currentElement = Expression.Variable(node.ItemType, "current");
            var didMoveNext = Expression.Variable(typeof(bool), "didMoveNext");
            var hasElements = Expression.Variable(typeof(bool), "hasElements");
            var enumerator = Expression.Variable(getEnumeratorMethod.ReturnType, "enumerator");

            //var task = Expression.Variable(typeof (Task));
            var exitLabel = Expression.Label();
            //var returnLabel = Expression.Label(typeof (Task));
            var checkNullLabel = Expression.Label();

            var collection = ParseExpression(node.Collection);
            if (collection.Type == typeof(object))
            {
                collection = Expression.Convert(collection, enumerableType);
            }

            this.PushScope(currentElement);
            var loopBody = HandleAsync(HandleNodeAsync(node.Body));

            var ex = Expression.Block(
                new[] { enumerator, hasElements },
                Expression.IfThen(Expression.IsTrue(CheckNull(collection)), Expression.Return(checkNullLabel)),
                Expression.Assign(hasElements, Expression.Constant(false)),
                Expression.Assign(enumerator, Expression.Call(collection, getEnumeratorMethod)),
                //Expression.Assign(task, Expression.Constant(Task.FromResult(false), typeof (Task))),
                Expression.Loop(Expression.Block(
                    new[] { didMoveNext },
                    Expression.Assign(didMoveNext, Expression.Call(enumerator, moveNextMethod)),
                    Expression.IfThenElse(Expression.IsFalse(didMoveNext),
                        Expression.Break(exitLabel),
                        Expression.Block(
                            new[] { currentElement },
                            Expression.Assign(currentElement, Expression.Property(enumerator, getCurrentMethod)),
                            Expression.Assign(hasElements, Expression.Constant(true)),
                            loopBody
                        )
                    )
                ), exitLabel),
                DisposeIfNeeded(enumerator),
                Expression.IfThen(Expression.IsFalse(hasElements), HandleNodeAsync(node.EmptyBody)),
                Expression.Label(checkNullLabel)//,
                //Expression.Return(returnLabel, task),
                //Expression.Label(returnLabel, Expression.Constant(null, typeof(Task)))
            );
            this.PopScope();

            return ex;
        }

		private Expression HandleIterate(IterateNode node)
		{
			if (node.Collection.ResultType.IsArray)
			{
				return HandleIterateArray(node);
			}

			var enumerableType = typeof(IEnumerable<>).MakeGenericType(node.ItemType);
			var getEnumeratorMethod = enumerableType.GetMethod("GetEnumerator");
			var getCurrentMethod = getEnumeratorMethod.ReturnType.GetProperty("Current").GetGetMethod();

			var currentElement = Expression.Variable(node.ItemType, "current");
			var didMoveNext = Expression.Variable(typeof(bool), "didMoveNext");
			var hasElements = Expression.Variable(typeof(bool), "hasElements");
			var enumerator = Expression.Variable(getEnumeratorMethod.ReturnType, "enumerator");

			//var task = Expression.Variable(typeof (Task));
			var exitLabel = Expression.Label();
            //var returnLabel = Expression.Label(typeof (Task));
            var checkNullLabel = Expression.Label();

            var collection = ParseExpression(node.Collection);
			if (collection.Type == typeof(object))
			{
				collection = Expression.Convert(collection, enumerableType);
			}

			this.PushScope(currentElement);
			var loopBody = HandleAsync(HandleNode(node.Body));

			var ex = Expression.Block(
				new[] { enumerator, hasElements },
                Expression.IfThen(Expression.IsTrue(CheckNull(collection)), Expression.Return(checkNullLabel)),
                Expression.Assign(hasElements, Expression.Constant(false)),
				Expression.Assign(enumerator, Expression.Call(collection, getEnumeratorMethod)),
				//Expression.Assign(task, Expression.Constant(Task.FromResult(false), typeof (Task))),
				Expression.Loop(Expression.Block(
					new[] { didMoveNext },
					Expression.Assign(didMoveNext, Expression.Call(enumerator, moveNextMethod)),
					Expression.IfThenElse(Expression.IsFalse(didMoveNext),
						Expression.Break(exitLabel),
						Expression.Block(
							new[] { currentElement },
							Expression.Assign(currentElement, Expression.Property(enumerator, getCurrentMethod)),
							Expression.Assign(hasElements, Expression.Constant(true)),
							loopBody
						)
					)
				), exitLabel),
				DisposeIfNeeded(enumerator),
                Expression.IfThen(Expression.IsFalse(hasElements), HandleNode(node.EmptyBody)),
                Expression.Label(checkNullLabel)//,
                //Expression.Return(returnLabel, task),
                //Expression.Label(returnLabel, Expression.Constant(null, typeof(Task)))
            );
			this.PopScope();

			return ex;
		}

        private static MethodCallExpression CheckNull(Expression value)
        {
            return Expression.Call(null, checkNullMethod, value);
        }

        private static MethodCallExpression CheckNotNull(string message, SyntaxTreeNode node, Expression value)
        {
            return Expression.Call(null, checkNotNullMethod, Expression.Constant(message), value, Expression.Constant(node));
        }

        private Expression HandleIterateArray(IterateNode node)
        {
            var index = Expression.Variable(typeof(int));
            var length = Expression.Variable(typeof(int));
            var currentElement = Expression.Variable(node.ItemType, "current");
            var exitLabel = Expression.Label();

            PushScope(currentElement);
            var body = HandleNode(node.Body);
            PopScope();

            var array = ParseExpression(node.Collection);
            var storedArray = Expression.Variable(array.Type);

            return Expression.Block(
                new[] { length, storedArray },
                Expression.Assign(storedArray, array),
                Expression.Assign(length, Expression.ArrayLength(storedArray)),
                Expression.IfThenElse(Expression.Equal(length, Expression.Constant(0)),
                    HandleNode(node.EmptyBody),
                    Expression.Block(
                        new[] { index },
                        Expression.Assign(index, Expression.Constant(0)),
                        Expression.Loop(Expression.Block(
                            new[] { currentElement },
                            Expression.Assign(currentElement, Expression.ArrayIndex(storedArray, index)),
                            body,
                            Expression.Assign(index, Expression.Increment(index)),
                            Expression.IfThen(Expression.Equal(index, length), Expression.Break(exitLabel))
                        ), exitLabel)
                    )
                )
            );
        }

        private static Expression DisposeIfNeeded(Expression instance)
        {
            if (!typeof(IDisposable).IsAssignableFrom(instance.Type))
            {
                return Expression.Empty();
            }
            return Expression.Call(instance, disposeMethod);
        }
    }
}