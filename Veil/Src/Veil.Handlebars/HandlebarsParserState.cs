using System;
using System.Collections.Generic;
using System.Linq;
using Veil.Compiler;
using Veil.Parser;

namespace Veil.Handlebars
{
    internal class HandlebarsParserState
    {
	    private readonly string _templateId;
	    private readonly IMemberLocator _memberLocator;
        public HandlebarsBlockStack BlockStack { get; private set; }

        public HandlebarsToken CurrentToken { get; private set; }

	    public SourceLocation CurrentLocation
	    {
			get { return new SourceLocation(_templateId, CurrentToken.Position, CurrentToken.Length); }
	    }

        public SyntaxTreeNode ExtendNode { get; set; }

        public bool TrimNextLiteral { get; set; }

        public bool ContinueProcessingToken { get; set; }

        public SyntaxTreeNode RootNode { get { return ExtendNode ?? BlockStack.GetCurrentBlockNode(); } }

        public HandlebarsParserState(string templateId, IMemberLocator memberLocator)
        {
	        _templateId = templateId;
	        _memberLocator = memberLocator;
            this.BlockStack = new HandlebarsBlockStack();
        }

        public void WriteLiteral(string s, SourceLocation location)
        {
            if (TrimNextLiteral)
            {
                s = s.TrimStart();
                TrimNextLiteral = false;
            }
            AddNodeToCurrentBlock(SyntaxTree.WriteString(s, location));
        }

        public ExpressionNode ParseExpression(string expression, SourceLocation location)
        {
            return HandlebarsExpressionParser.Parse(this, BlockStack, location, expression, _memberLocator);
        }

        internal void SetCurrentToken(HandlebarsToken token)
        {
            CurrentToken = token;
            ContinueProcessingToken = false;
        }

        internal SyntaxTreeNode AddNodeToCurrentBlock(SyntaxTreeNode node)
        {
            BlockStack.Peek().Block.Add(node);
            return node;
        }

        internal SyntaxTreeNode LastNode()
        {
            return BlockStack.GetCurrentBlockNode().LastNode();
        }
    }
}