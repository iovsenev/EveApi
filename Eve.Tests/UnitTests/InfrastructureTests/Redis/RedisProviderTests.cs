using Eve.Domain.Common;
using Eve.Infrastructure.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;

namespace Eve.Tests.UnitTests.InfrastructureTests.Redis;
public class RedisProviderTests
{
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly RedisProvider _redisProvider;

    public RedisProviderTests()
    {
        _mockCache = new();
        _redisProvider = new RedisProvider(_mockCache.Object);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDeserializedEntity_WhenKeyExist()
    {
        //Arrange

        var key = "testKey";
        var expectedObject = new TestClass { Name = "Test", Value = 123 };
        var serializedObject = JsonSerializer.Serialize(expectedObject);

        _mockCache
            .Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(serializedObject));

        //Act
        var result = await _redisProvider.GetAsync<TestClass>(key, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(expectedObject.Name, result.Name);
        Assert.Equal(expectedObject.Value, result.Value);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        // arrange
        var key = "nonexistentKey";

        _mockCache
            .Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null);

        //Act
        var result = await _redisProvider.GetAsync<object>(key, CancellationToken.None);

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_StoresSerializedObjectInCache()
    {
        // Arrange
        var key = "test_key";
        var testObject = new TestClass { Name = "TestName" };
        var serializedObject = JsonSerializer.Serialize(testObject);

        _mockCache
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((k, v, o, t) =>
            {
                Assert.Equal(key, k);

                var deserializedObject = JsonSerializer.Deserialize<TestClass>(v);
                Assert.NotNull(deserializedObject);
                Assert.Equal(testObject.Name, deserializedObject?.Name);
            })
            .Returns(Task.CompletedTask);

        // Act
        await _redisProvider.SetAsync(key, testObject, CancellationToken.None);

        // Assert
        _mockCache.Verify(
            c => c.SetAsync(
                key,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        // Arrange
        var key = null as string;  // key = null
        var testObject = new TestClass { Name = "TestName" };

        _mockCache
            .Setup(c => c.SetAsync(
                It.Is<string>(s => s == null),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException("key"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _redisProvider.SetAsync(key, testObject, CancellationToken.None));

        Assert.Equal("key", exception.ParamName);
    }

    [Fact]
    public async Task RemoveAsync_RemovesKeyFromCache()
    {
        // Arrange
        var key = "key_to_remove";

        // Act
        await _redisProvider.RemoveAsync(key, CancellationToken.None);

        // Assert
        _mockCache.Verify(c => c.RemoveAsync(
            key,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetOrSetAsync_ReturnsCachedValue_WhenKeyExists()
    {
        // Arrange
        var key = "existing_key";
        var cachedValue = new { Name = "CachedValue" };
        var serializedValue = JsonSerializer.Serialize(cachedValue);

        _mockCache
            .Setup(c => c.GetAsync(
                key,
                It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Encoding.UTF8.GetBytes(serializedValue));

        // Act
        var result = await _redisProvider.GetOrSetAsync(
            key,
            async () => await Result(true),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal("CachedValue", result.Value.Name.ToString());
    }

    [Fact]
    public async Task GetOrSetAsync_SetsValue_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "nonexistent_key";
        var newValue = new { Name = "NewValue" };

        _mockCache.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
                  .ReturnsAsync((byte[])null);

        // Act
        var result = await _redisProvider.GetOrSetAsync(
            key,
            async () => await Result(true),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal("NewValue", result.Value.Name.ToString());
        _mockCache.Verify(c => c.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private async Task<Result<TestClass>> Result(bool isSuccess)
    {
        if (isSuccess)
            return new TestClass { Name = "NewValue", Value = 1234 };
        return Error.InternalServer();
    }
}

class TestClass
{
    public string Name { get; set; }
    public int Value { get; set; }
}
