using Microsoft.Practices.Unity;

namespace NitroNet.UnityModules
{
	public interface IUnityModule
	{
		void Configure(IUnityContainer container);
	}
}