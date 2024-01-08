using Microsoft.Extensions.Caching.Memory;

namespace TestAPI6
{
    public class Caching
    {
        private readonly IMemoryCache _memoryCache;

        public Caching(IMemoryCache memoryCache) =>
            _memoryCache = memoryCache;
    }
}
