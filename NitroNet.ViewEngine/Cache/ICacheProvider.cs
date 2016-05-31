using System;

namespace NitroNet.ViewEngine.Cache
{
	public interface ICacheProvider
	{
		void Set<TValue>(string key, TValue value, DateTimeOffset offset);
		bool TryGet<TValue>(string key, out TValue value) where TValue : class;
	}
}
