using System;
using System.Collections.Generic;

namespace Veil.Helper
{
    [Obsolete("Use IHandlebarsNetHelperHandlerFactory instead")]
    public interface IHelperHandlerFactory
	{
		IEnumerable<IHelperHandler> Create();
	}
}