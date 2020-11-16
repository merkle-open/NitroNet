using System.Collections.Generic;
using NitroNet.ViewEngine.Context;

namespace Veil.Helper
{
	public interface IBlockHelperHandler : IHelperHandler
	{
		void Leave(object model, RenderingContext context, string name, IDictionary<string, string> parameters);
	}
}