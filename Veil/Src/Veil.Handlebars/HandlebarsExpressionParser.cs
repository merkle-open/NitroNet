using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Veil.Compiler;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal static class HandlebarsExpressionParser
    {
        public static ExpressionNode Parse(HandlebarsParserState state, HandlebarsBlockStack blockStack, SourceLocation location, string expression, IMemberLocator memberLocator = null)
        {
            int recursionLevel = 0;

	        memberLocator = memberLocator ?? MemberLocator.Default;

            expression = expression.Trim();

            if (expression == "this")
            {
                return SyntaxTreeExpression.Self(blockStack.GetCurrentModelType(), location);
            }
            if (expression.StartsWith("../"))
            {
                var blockNode = blockStack.FirstNode();
                while (expression.StartsWith("../"))
                {
                    var parentBlockNode = blockStack.GetParentNode(blockNode);
                    if (parentBlockNode != null)
                    {
                        blockNode = parentBlockNode;
                        expression = expression.Substring(3);
                        location = location.MoveIndex(3);
                    }
                    recursionLevel++;
                }
                return ParseAgainstModel(blockStack.GetCurrentModelType(blockNode), expression, ExpressionScope.ModelOfParentScope, recursionLevel, memberLocator, location);
            }

            return ParseAgainstModel(blockStack.GetCurrentModelType(), expression, ExpressionScope.CurrentModelOnStack, recursionLevel, memberLocator, location);
        }

		private static ExpressionNode ParseAgainstModel(Type modelType, string expression, ExpressionScope expressionScope, int recursionLevel, IMemberLocator memberLocator, SourceLocation location)
        {
            var dotIndex = expression.IndexOf('.');
            if (dotIndex >= 0)
            {
				var subModel = ParseAgainstModel(modelType, expression.Substring(0, dotIndex), expressionScope, recursionLevel, memberLocator, location.SetLength(dotIndex));
                return SyntaxTreeExpression.SubModel(
                    subModel,
                    ParseAgainstModel(subModel.ResultType, expression.Substring(dotIndex + 1), ExpressionScope.CurrentModelOnStack, 0, memberLocator, location.MoveIndex(dotIndex + 1)),
					location
                );
            }

            if (expression.EndsWith("()"))
            {
                var func = memberLocator.FindMember(modelType, expression.Substring(0, expression.Length - 2), MemberTypes.Method);
                if (func != null) return SyntaxTreeExpression.Function(modelType, func.Name, location, expressionScope);
            }

            var prop = memberLocator.FindMember(modelType, expression, MemberTypes.Property | MemberTypes.Field);
            if (prop != null)
            {
                switch (prop.MemberType)
                {
                    case MemberTypes.Property: return SyntaxTreeExpression.Property(modelType, prop.Name, location, expressionScope, recursionLevel);
                    case MemberTypes.Field: return SyntaxTreeExpression.Field(modelType, prop.Name, location, expressionScope, recursionLevel);
                }
            }

            if (IsLateBoundAcceptingType(modelType))
                return SyntaxTreeExpression.LateBound(expression, location, memberLocator, false, expressionScope, recursionLevel);

            throw new VeilParserException(String.Format("Unable to parse model expression '{0}' againt model '{1}'", expression, modelType.Name), location);
        }

        private static bool IsLateBoundAcceptingType(Type type)
        {
            return type == typeof(object) 
                || type.IsDictionary()
                || type.GetInterfaces().Any(IsDictionary)
                || type.GetProperties().Any(p => p.GetIndexParameters().Any());
        }

        private static bool IsDictionary(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }
    }
}