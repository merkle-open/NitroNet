using System;
using System.Runtime.Caching;

namespace NitroNet.ViewEngine.Cache
{
	public class MemoryCacheProvider : ICacheProvider
	{
		private readonly MemoryCache _cache = MemoryCache.Default;

		public void Set<TValue>(string key, TValue value, DateTimeOffset offset)
		{
			_cache.Set(key, value, new CacheItemPolicy
			{
				AbsoluteExpiration = DateTimeOffset.Now.AddHours(24),
				Priority = CacheItemPriority.NotRemovable
			});
		}

		public bool TryGet<TValue>(string key, out TValue value) where TValue : class
		{
			value = _cache.Get(key) as TValue;
			return value != null;
		}
	}
}