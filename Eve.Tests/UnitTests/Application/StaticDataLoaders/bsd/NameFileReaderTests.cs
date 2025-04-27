using Eve.Application.StaticDataLoaders.Common;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.bsd;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eve.Tests.UnitTests.Application.StaticDataLoaders.bsd;
public class NameFileReaderTests
{
    private readonly Mock<IFileReader> _readerMock;
    private readonly Mock<ILogger<NamesFileReader>> _loggerMock;
    private readonly NamesFileReader _reader;

    public NameFileReaderTests()
    {
        _readerMock = new();
        _loggerMock = new();
        _reader = new NamesFileReader(_readerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetData_ReturnsSuccess()
    {
        //arrange
        var _readerReturns = new List<Dictionary<string, object>>();
        var nameObject = new Dictionary<string, object>();
        nameObject.Add("itemID", "1");
        nameObject.Add("itemName", "name");
        _readerReturns.Add(nameObject);

        _readerMock
            .Setup(r => r.ReadYamlFileBSD(It.IsAny<string>()))
            .ReturnsAsync(_readerReturns);

        //act
        var result = await _reader.GetData("asdfa");

        //assert
        result.Should().NotBeNull();
        result.First().Name.Should().Be("name");
        result.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetData_ReturnsSuccess_WhenFileReaderReturnsEmptyCollection()
    {
        //arrange
        var _readerReturns = new List<Dictionary<string, object>>();

        _readerMock
            .Setup(r => r.ReadYamlFileBSD(It.IsAny<string>()))
            .ReturnsAsync(_readerReturns);

        //act
        var result = await _reader.GetData("asdfa");

        //assert
        result.Should().NotBeNull();
        result.Any().Should().BeFalse();
    }

    [Fact]
    public async Task GetData_ReturnsArgumentNullException_WhenEmptyPath()
    {
        //arrange
        var _readerReturns = new List<Dictionary<string, object>>();
        var nameObject = new Dictionary<string, object>();
        nameObject.Add("itemID", "1");
        nameObject.Add("itemName", "name");
        _readerReturns.Add(nameObject);

        _readerMock
            .Setup(r => r.ReadYamlFileBSD(It.IsAny<string>()))
            .ReturnsAsync(_readerReturns);

        //act
        await Assert.ThrowsAsync<ArgumentNullException>( () => _reader.GetData(""));
    }
}
