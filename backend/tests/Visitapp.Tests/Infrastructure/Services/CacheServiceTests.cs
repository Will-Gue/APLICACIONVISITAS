using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Visitapp.Infrastructure.Services;
using Xunit;

namespace Visitapp.Tests.Infrastructure.Services
{
    public class CacheServiceTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILogger<CacheService>> _mockLogger;
        private readonly CacheService _cacheService;

        public CacheServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _mockLogger = new Mock<ILogger<CacheService>>();
            _cacheService = new CacheService(_memoryCache, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAsync_ExistingKey_ReturnsValue()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            _memoryCache.Set(key, value);

            // Act
            var result = await _cacheService.GetAsync<string>(key);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task GetAsync_NonExistingKey_ReturnsNull()
        {
            // Arrange
            var key = "non-existing-key";

            // Act
            var result = await _cacheService.GetAsync<string>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrCreateAsync_ExistingKey_ReturnsCachedValue()
        {
            // Arrange
            var key = "existing-key";
            var cachedValue = "cached-value";
            var factoryCallCount = 0;
            
            _memoryCache.Set(key, cachedValue);

            // Act
            var result = await _cacheService.GetOrCreateAsync(key, () =>
            {
                factoryCallCount++;
                return Task.FromResult("factory-value");
            });

            // Assert
            Assert.Equal(cachedValue, result);
            Assert.Equal(0, factoryCallCount); // Factory should not be called
        }

        [Fact]
        public async Task GetOrCreateAsync_NonExistingKey_CallsFactoryAndCaches()
        {
            // Arrange
            var key = "new-key";
            var factoryValue = "factory-value";
            var factoryCallCount = 0;

            // Act
            var result = await _cacheService.GetOrCreateAsync(key, () =>
            {
                factoryCallCount++;
                return Task.FromResult(factoryValue);
            });

            // Assert
            Assert.Equal(factoryValue, result);
            Assert.Equal(1, factoryCallCount);
            
            // Verify it was cached
            var cachedResult = await _cacheService.GetAsync<string>(key);
            Assert.Equal(factoryValue, cachedResult);
        }

        [Fact]
        public async Task SetAsync_ValidKeyValue_StoresInCache()
        {
            // Arrange
            var key = "set-key";
            var value = "set-value";

            // Act
            await _cacheService.SetAsync(key, value);

            // Assert
            var result = await _cacheService.GetAsync<string>(key);
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task SetAsync_WithExpiration_ExpiresCorrectly()
        {
            // Arrange
            var key = "expiring-key";
            var value = "expiring-value";
            var expiration = TimeSpan.FromMilliseconds(100);

            // Act
            await _cacheService.SetAsync(key, value, expiration);

            // Assert - immediately should be available
            var result1 = await _cacheService.GetAsync<string>(key);
            Assert.Equal(value, result1);

            // Wait for expiration
            await Task.Delay(150);

            // Should be expired now
            var result2 = await _cacheService.GetAsync<string>(key);
            Assert.Null(result2);
        }

        [Fact]
        public async Task RemoveAsync_ExistingKey_RemovesFromCache()
        {
            // Arrange
            var key = "remove-key";
            var value = "remove-value";
            await _cacheService.SetAsync(key, value);

            // Verify it exists
            var beforeRemove = await _cacheService.GetAsync<string>(key);
            Assert.Equal(value, beforeRemove);

            // Act
            await _cacheService.RemoveAsync(key);

            // Assert
            var afterRemove = await _cacheService.GetAsync<string>(key);
            Assert.Null(afterRemove);
        }

        [Fact]
        public async Task RemoveByPatternAsync_LogsWarning()
        {
            // Arrange
            var pattern = "test-*";

            // Act
            await _cacheService.RemoveByPatternAsync(pattern);

            // Assert - verify warning was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Pattern removal not fully supported")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}