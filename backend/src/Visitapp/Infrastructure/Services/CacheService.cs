using Microsoft.Extensions.Caching.Memory;
using Visitapp.Application.Interfaces;

namespace Visitapp.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                return _cache.Get<T>(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache key: {Key}", key);
                return null;
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
        {
            try
            {
                if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
                {
                    return cachedValue;
                }

                var value = await factory();
                
                var options = new MemoryCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration.Value;
                }
                else
                {
                    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // Default 30 minutes
                }

                _cache.Set(key, value, options);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrCreateAsync for key: {Key}", key);
                return await factory(); // Fallback to direct call
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration.Value;
                }
                else
                {
                    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                }

                _cache.Set(key, value, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                // Note: IMemoryCache doesn't support pattern removal natively
                // This is a simplified implementation
                _logger.LogWarning("Pattern removal not fully supported in MemoryCache: {Pattern}", pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache pattern: {Pattern}", pattern);
            }
        }
    }
}