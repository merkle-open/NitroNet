using System;

namespace NitroNet.ViewEngine.IO
{
	public interface IFileSystemSubscription : IDisposable
	{
		void Register(Action<string> handler);
	}
}