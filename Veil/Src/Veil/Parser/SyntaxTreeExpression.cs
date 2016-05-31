using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Compiler;
using Veil.Helper;
using Veil.Parser.Nodes;

namespace Veil.Parser
{
	/// <summary>
	/// Factory methods for create expression nodes
	/// </summary>
	public static class SyntaxTreeExpression
	{
	    /// <summary>
	    /// Evaluate a property on the model object
	    /// </summary>
	    /// <param name="modelType">The type of the scoped model</param>
	    /// <param name="propertyName">The name of the property</param>
	    /// <param name="location"></param>
	    /// <param name="scope">The scope this expression evaluated in</param>
	    /// <param name="recursionLevel"></param>
	    public static PropertyExpressionNode Property(Type modelType, string propertyName, SourceLocation location, ExpressionScope scope = ExpressionScope.CurrentModelOnStack, int recursionLevel = 0)
		{
			return new PropertyExpressionNode
			{
				Location = location,
				PropertyInfo = modelType.GetProperty(propertyName),
				Scope = scope,
                RecursionLevel = recursionLevel
			};
		}

	    /// <summary>
	    /// Evaluate a field on the model object
	    /// </summary>
	    /// <param name="modelType">The type of the scoped model</param>
	    /// <param name="fieldName">The name of the field</param>
	    /// <param name="location"></param>
	    /// <param name="scope">The scope this expression evaluated in</param>
	    /// <param name="recursionLevel"></param>
	    public static FieldExpressionNode Field(Type modelType, string fieldName, SourceLocation location, ExpressionScope scope = ExpressionScope.CurrentModelOnStack, int recursionLevel = 0)
		{
			return new FieldExpressionNode
			{
				Location = location,
				FieldInfo = modelType.GetField(fieldName),
				Scope = scope,
                RecursionLevel = recursionLevel
			};
		}

	    /// <summary>
	    /// Evaluate an expression on a sub model, can be nested to traverse any depth of sub models
	    /// </summary>
	    /// <param name="modelExpression">An expression referencing the model to traverse to</param>
	    /// <param name="subModelExpression">An expression to evaluate in the scope of the model that has been traversed to</param>
	    /// <param name="location"></param>
	    /// <param name="scope">The scope this expression evaluated in</param>
	    public static SubModelExpressionNode SubModel(ExpressionNode modelExpression, ExpressionNode subModelExpression, SourceLocation location, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
		{
			return new SubModelExpressionNode
			{
				Location = location,
				ModelExpression = modelExpression,
				SubModelExpression = subModelExpression,
				Scope = scope
			};
		}

	    /// <summary>
	    /// Evaluate a function call on the model
	    /// </summary>
	    /// <param name="modelType">The type of the scoped model</param>
	    /// <param name="functionName">The name of the function</param>
	    /// <param name="location"></param>
	    /// <param name="scope">The scope this expression evaluated in</param>
	    public static FunctionCallExpressionNode Function(Type modelType, string functionName, SourceLocation location, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
		{
			return new FunctionCallExpressionNode
			{
				Location = location,
				MethodInfo = modelType.GetMethod(functionName, new Type[0]),
				Scope = scope
			};
		}

	    /// <summary>
	    /// Evaluate the model itself e.g. Value types
	    /// </summary>
	    /// <param name="modelType">The type of the scoped model</param>
	    /// <param name="location"></param>
	    /// <param name="scope">The scope this expression evaluated in</param>
	    public static SelfExpressionNode Self(Type modelType, SourceLocation location, ExpressionScope scope = ExpressionScope.CurrentModelOnStack)
		{
			return new SelfExpressionNode
			{
				Location = location,
				ModelType = modelType,
				Scope = scope
			};
		}

	    /// <summary>
	    /// Evaluate whether the collectionExpression has Count > 0
	    /// Can only be used on types that implement <see cref="System.Collections.ICollection"/>
	    /// </summary>
	    /// <param name="collectionExpression">An expression referencing a Collection</param>
	    /// <param name="location"></param>
	    /// <param name="recursionLevel"></param>
	    public static CollectionHasItemsExpressionNode HasItems(ExpressionNode collectionExpression, SourceLocation location, int recursionLevel = 0)
		{
			return new CollectionHasItemsExpressionNode
			{
				Location = location,
				CollectionExpression = collectionExpression,
				Scope = collectionExpression.Scope,
                RecursionLevel = recursionLevel
			};
		}

	    /// <summary>
	    /// Evaluate a property at runtime against an unknown model type
	    /// </summary>
	    /// <param name="itemName">The name of the proeprty that will be searched for</param>
	    /// <param name="memberLocator"></param>
	    /// <param name="isCaseSensitive">Indcates whether the expression should be evaluated with case sensitivity</param>
	    /// <param name="scope">The scope this expression evaluated in</param>
	    /// <param name="location"></param>
	    /// <param name="recursionLevel"></param>
	    public static LateBoundExpressionNode LateBound(string itemName, SourceLocation location, IMemberLocator memberLocator = null, bool isCaseSensitive = true, ExpressionScope scope = ExpressionScope.CurrentModelOnStack, int recursionLevel = 0)
		{
			return new LateBoundExpressionNode
			{
				Location = location,
                MemberLocator = memberLocator ?? MemberLocator.Default,
				ItemName = itemName,
				Scope = scope,
                RecursionLevel = recursionLevel
			};
		}

		public static HelperExpressionNode Helper(string expression, IHelperHandler helperHandler, SourceLocation location)
		{
			var parts = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			return Helper(parts, helperHandler, location);
		}

        public static HelperExpressionNode Helper(string[] parameter, IHelperHandler helperHandler, SourceLocation location)
		{
			var data = new Dictionary<string, string>();

			if (parameter.Length > 1)
			{
				foreach (var value in parameter.Skip(1))
				{
					var tmp = value.Split(new[] {'='}, 2);
					data.Add(tmp[0], tmp.Length == 2 ? tmp[1] : string.Empty);
				}
			}

			return new HelperExpressionNode
			{
				Location = location,
				Name = parameter[0],
				Parameters = data,
                HelperHandler = helperHandler
			};
		}
	}
}