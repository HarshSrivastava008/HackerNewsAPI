using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace NewsAPI.Tests
{
    public class CustomMemoryCache : IMemoryCache
    {
        private readonly Dictionary<object, object> _cache = new Dictionary<object, object>();
        public ICacheEntry CreateEntry(object key)
        {
            var entry = new Mock<ICacheEntry>();
            entry.SetupAllProperties();
            entry.Setup(e => e.Key).Returns(key);
            _cache[key] = entry.Object;
            return entry.Object;
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Dispose()
        {
            _cache.Clear();
        }
    }
}
