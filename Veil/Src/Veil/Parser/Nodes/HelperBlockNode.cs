namespace Veil.Parser.Nodes
{
	public class HelperBlockNode : SyntaxTreeNode
	{
		public HelperExpressionNode HelperExpression { get; set; }
		public BlockNode Block { get; set; }
	}
}
