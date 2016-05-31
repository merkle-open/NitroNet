using System.Collections.Generic;
using System.Threading.Tasks;

namespace NitroNet.ViewEngine
{
	public interface ITemplateRepository
	{
		Task<TemplateInfo> GetTemplateAsync(string id);

        Task<IEnumerable<TemplateInfo>> GetTemplatesByNameAsync(string name);

        IEnumerable<TemplateInfo> GetAll();
    }
}