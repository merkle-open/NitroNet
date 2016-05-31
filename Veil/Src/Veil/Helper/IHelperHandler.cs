using System.Collections.Generic;
using System.Threading.Tasks;

namespace Veil.Helper
{
	public interface IHelperHandler
	{
		bool IsSupported(string name);
		void Evaluate(object model, RenderingContext context, IDictionary<string, string> parameters);
	}
}