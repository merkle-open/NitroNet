using System;

namespace NitroNet.ViewEngine.Cache
{
	public class NullCacheProvider : ICacheProvider
	{
		public void Set<TValue>(string key, TValue value, DateTimeOffset offset)
		{
		}

		public bool TryGet<TValue>(string key, out TValue value) where TValue : class
		{
			value = default(TValue);
			return false;
		}
	}
}