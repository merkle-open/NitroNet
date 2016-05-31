using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veil.Compiler;
using Veil.Helper;
using Veil.Parser;
using Veil.Parser.Nodes;

namespace Veil.Handlebars
{
    /// <summary>
    /// Veil parser for the Handlebars syntax
    /// </summary>
    public class HandlebarsParser : ITemplateParser
    {
        private const string OverrideSectionName = "body";

        private static readonly Dictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>> SyntaxHandlers = new Dictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>>
        {
            { x => !x.IsSyntaxToken, HandleStringLiteral },
            { x => x.TrimLastLiteral, HandleTrimLastLiteral },
            { x => x.TrimNextLiteral, HandleTrimNextLiteral },
            { x => x.Content.StartsWith("!"), x => { /* Ignore Comments */ }},
            { x => x.Content.StartsWith("#if"), HandleIf },
            { x => x.Content == "else", HandleElse },
            { x => x.Content == "/if", HandleEndIf },
            { x => x.Content.StartsWith("#unless"), HandleUnless },
            { x => x.Content == "/unless", HandleEndUnless },
            { x => x.Content.StartsWith("#each"), HandleEach },
            { x => x.Content == "/each", HandleEndEach },
            { x => x.Content == "#flush", HandleFlush },
            { x => x.Content.StartsWith("#with"), HandleWith },
            { x => x.Content == "/with", HandleEndWith },
            { x => x.Content.StartsWith(">"), HandlePartial },
            { x => x.Content.StartsWith("<"), HandleMaster },
            { x => x.Content == "body", HandleBody }
        };

	    private static readonly Dictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>> SyntaxHandlersAfter = new Dictionary
		    <Func<HandlebarsToken, bool>, Action<HandlebarsParserState>>
	    {
			{ x => true, HandleExpression }
	    };

		public SyntaxTreeNode Parse(string templateId, TextReader templateReader, Type modelType, IMemberLocator memberLocator, IHelperHandler[] helperHandlers)
        {
            if (memberLocator == null)
                memberLocator = MemberLocator.Default;

            var state = new HandlebarsParserState(templateId, memberLocator);
            var tokens = HandlebarsTokenizer.Tokenize(templateReader);
            state.BlockStack.PushNewBlockWithModel(modelType, new SourceLocation(templateId, 0, 0));

            var helpers = SyntaxHandlers.Union(GetHelperHandlers(helperHandlers ?? Enumerable.Empty<IHelperHandler>())).Union(SyntaxHandlersAfter).ToList();

            foreach (var token in tokens)
            {
                state.SetCurrentToken(token);

                foreach (var handler in helpers)
                {
                    if (handler.Key(token))
                    {
                        handler.Value.Invoke(state);
                        if (state.ContinueProcessingToken)
                        {
                            state.ContinueProcessingToken = false;
                            continue;
                        }
                        break;
                    }
                }
            }

            state.AssertStackOnRootNode();

            return state.RootNode;
        }

	    private static IDictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>> GetHelperHandlers(IEnumerable<IHelperHandler> helperHandlers)
	    {
		    var dict = new Dictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>>();
		    foreach (var helper in helperHandlers)
		    {
			    var innerDict = GetHelperHandlers(helper);
			    foreach (var entry in innerDict)
				    dict.Add(entry.Key, entry.Value);
		    }
			return dict;
	    }

	    private static IDictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>> GetHelperHandlers(IHelperHandler helper)
	    {
		    var blockHelper = helper as IBlockHelperHandler;
		    if (blockHelper != null)
		    {
				return new Dictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>>
				{
					{x => x.Content.StartsWith("#") && helper.IsSupported(x.Content.Substring(1)), state => HandleHelperStart(state, blockHelper)},
					{x => x.Content.StartsWith("/") && helper.IsSupported(x.Content.Substring(1)), state => HandleHelperEnd(state, blockHelper)}
				};
		    }

		    return new Dictionary<Func<HandlebarsToken, bool>, Action<HandlebarsParserState>>
		    {
				{x => helper.IsSupported(x.Content), state => HandleHelper(state, helper)}
		    };
	    }

		private static void HandleHelperEnd(HandlebarsParserState state, IBlockHelperHandler helper)
	    {
			// TODO: Stack validation
			state.BlockStack.PopBlock();
	    }

	    private static void HandleHelperStart(HandlebarsParserState state, IBlockHelperHandler helper)
	    {
			var block = SyntaxTree.Block(state.CurrentLocation);
			var helperBlock = SyntaxTree.Helper(SyntaxTreeExpression.Helper(state.CurrentToken.Content.Substring(1), helper, state.CurrentLocation), block, state.CurrentLocation);
			state.AddNodeToCurrentBlock(helperBlock);
			state.BlockStack.PushModelInheritingBlock(block);
	    }

	    private static void HandleHelper(HandlebarsParserState state, IHelperHandler helper)
	    {
		    string expression = state.CurrentToken.Content;
			state.AddNodeToCurrentBlock(SyntaxTreeExpression.Helper(expression, helper, state.CurrentLocation));
	    }

	    private static void HandleStringLiteral(HandlebarsParserState state)
        {
            state.WriteLiteral(state.CurrentToken.Content, state.CurrentLocation);
        }

        private static void HandleTrimLastLiteral(HandlebarsParserState state)
        {
            var literal = state.LastNode() as WriteLiteralNode;
            if (literal != null)
            {
                literal.LiteralContent = literal.LiteralContent.TrimEnd();
            }
            state.ContinueProcessingToken = true;
        }

        private static void HandleTrimNextLiteral(HandlebarsParserState state)
        {
            state.TrimNextLiteral = true;
            state.ContinueProcessingToken = true;
        }

        private static void HandleIf(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block(state.CurrentLocation);
            var conditional = SyntaxTree.Conditional(state.ParseExpression(state.CurrentToken.Content.Substring(4), state.CurrentLocation.MoveIndex(4)), state.CurrentLocation, block);
            state.AddNodeToCurrentBlock(conditional);
            state.BlockStack.PushModelInheritingBlock(block);
        }

        private static void HandleElse(HandlebarsParserState state)
        {
            state.AssertInsideConditionalOrIteration("{{else}}");
            if (state.IsInEachBlock())
            {
                HandleIterationElse(state);
            }
            else
            {
                HandleConditionalElse(state);
            }
        }

        private static void HandleIterationElse(HandlebarsParserState state)
        {
            var elseBlock = state.BlockStack.GetCurrentBlockContainer<IterateNode>().EmptyBody;
            state.BlockStack.PopBlock();
            state.BlockStack.PushModelInheritingBlock(elseBlock);
        }

        private static void HandleConditionalElse(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block(state.CurrentLocation);
            state.BlockStack.GetCurrentBlockContainer<ConditionalNode>().FalseBlock = block;
            state.BlockStack.PopBlock();
            state.BlockStack.PushModelInheritingBlock(block);
        }

        private static void HandleEndIf(HandlebarsParserState state)
        {
            state.AssertInsideConditional("{{/if}}");
            state.BlockStack.PopBlock();
        }

        private static void HandleUnless(HandlebarsParserState state)
        {
            var block = SyntaxTree.Block(state.CurrentLocation);
            var conditional = SyntaxTree.Conditional(state.ParseExpression(state.CurrentToken.Content.Substring(8), state.CurrentLocation.MoveIndex(8)), state.CurrentLocation, SyntaxTree.Block(state.CurrentLocation), block);
            state.AddNodeToCurrentBlock(conditional);
            state.BlockStack.PushModelInheritingBlock(block);
        }

        private static void HandleEndUnless(HandlebarsParserState state)
        {
            state.AssertInsideConditional("{{/unless}}");
            state.BlockStack.PopBlock();
        }

        private static void HandleEach(HandlebarsParserState state)
        {
            var iteration = SyntaxTree.Iterate(
                state.ParseExpression(state.CurrentToken.Content.Substring(6), state.CurrentLocation.MoveIndex(6)),
				state.CurrentLocation,
                SyntaxTree.Block(state.CurrentLocation)
            );
            state.AddNodeToCurrentBlock(iteration);
            state.BlockStack.PushBlock(new HandlebarsParserBlock { Block = iteration.Body, ModelInScope = iteration.ItemType });
        }

        private static void HandleEndEach(HandlebarsParserState state)
        {
            state.AssertInsideIteration("{{/each}}");
            state.BlockStack.PopBlock();
        }

        private static void HandleFlush(HandlebarsParserState state)
        {
            state.AddNodeToCurrentBlock(SyntaxTree.Flush(state.CurrentLocation));
        }

        private static void HandleWith(HandlebarsParserState state)
        {
            var withBlock = SyntaxTree.Block(state.CurrentLocation);
            var modelExpression = state.ParseExpression(state.CurrentToken.Content.Substring(6), state.CurrentLocation.MoveIndex(6));
            state.AddNodeToCurrentBlock(SyntaxTree.ScopeNode(modelExpression, withBlock, state.CurrentLocation));
            state.BlockStack.PushBlock(new HandlebarsParserBlock
            {
                Block = withBlock,
                ModelInScope = modelExpression.ResultType
            });
        }

        private static void HandleEndWith(HandlebarsParserState state)
        {
            state.AssertInsideWith("{{/with}}");
            state.BlockStack.PopBlock();
        }

        private static void HandlePartial(HandlebarsParserState state)
        {
            var partialTemplateName = state.CurrentToken.Content.Substring(1).Trim();
            var self = SyntaxTreeExpression.Self(state.BlockStack.GetCurrentModelType(), state.CurrentLocation);
            state.AddNodeToCurrentBlock(SyntaxTree.Include(partialTemplateName, self, state.CurrentLocation));
        }

        private static void HandleMaster(HandlebarsParserState state)
        {
            state.AssertSyntaxTreeIsEmpty("There can be no content before a {{< }} expression.");
            var masterTemplateName = state.CurrentToken.Content.Substring(1).Trim();
            state.ExtendNode = SyntaxTree.Extend(masterTemplateName, state.CurrentLocation, new Dictionary<string, SyntaxTreeNode>
            {
                { OverrideSectionName, state.BlockStack.GetCurrentBlockNode() }
            });
        }

        private static void HandleBody(HandlebarsParserState state)
        {
            state.AddNodeToCurrentBlock(SyntaxTree.Override(OverrideSectionName, state.CurrentLocation));
        }

        private static void HandleExpression(HandlebarsParserState state)
        {
            var expression = state.ParseExpression(state.CurrentToken.Content, state.CurrentLocation);
            state.AddNodeToCurrentBlock(SyntaxTree.WriteExpression(expression, state.CurrentLocation, state.CurrentToken.IsHtmlEscape));
        }
    }
}