using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Veil.Parser.Nodes;

namespace Veil.Compiler
{
	internal partial class VeilTemplateCompiler<T>
	{
		private Expression HandleBlockAsync(BlockNode block)
		{
			if (!block.Nodes.Any())
			{
				return Expression.Empty();
			}

			var blockNodes = (from node in block.Nodes
							  select this.HandleNodeAsync(node)).ToArray();

			return HandleBlockAsync(blockNodes);
		}

		private Expression HandleBlockAsync(params Expression[] blockNodes)
		{
			return Expression.Block(blockNodes.Select(HandleAsync).ToList());
		}

		private Expression HandleBlock(BlockNode block)
		{
			if (!block.Nodes.Any())
			{
				return Expression.Empty();
			}

			var blockNodes = (from node in block.Nodes
							  select this.HandleNode(node)).ToArray();

			return HandleBlock(blockNodes);
		}

		private Expression HandleBlock(params Expression[] blockNodes)
		{
			return Expression.Block(blockNodes.Select(HandleAsync).ToList());
		}
	}
}