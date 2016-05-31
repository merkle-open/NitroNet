using System.Collections.Generic;

namespace Veil.Helper
{
	public interface IHelperHandlerFactory
	{
		IEnumerable<IHelperHandler> Create();
	}
}