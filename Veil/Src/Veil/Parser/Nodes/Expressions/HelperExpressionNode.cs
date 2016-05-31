using System;
using System.Collections.Generic;
using Veil.Helper;

namespace Veil.Parser.Nodes
{
	public class HelperExpressionNode : ExpressionNode
	{
		public override Type ResultType
		{
			get { return typeof(object); }
		}

		public string Name { get; set; }
		public IDictionary<string, string> Parameters { get; set; }

        public IHelperHandler HelperHandler { get; set; }
	}
}
