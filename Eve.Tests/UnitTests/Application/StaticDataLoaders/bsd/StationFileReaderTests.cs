using Eve.Application.StaticDataLoaders.Common;
using Eve.Application.StaticDataLoaders.ConvertFromYaml.bsd;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eve.Tests.UnitTests.Application.StaticDataLoaders.bsd;
public class StationFileReaderTests
{
    private readonly Mock<IFileReader> _readerMock;
    private readonly Mock<ILogger<StationFileReader>> _loggerMock;
    private readonly StationFileReader _reader;

    public StationFileReaderTests()
    {
        _readerMock = new();
        _loggerMock = new();
        _reader = new StationFileReader(_readerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetData_ReturnsSuccess()
    {
        //arrange
        var _readerReturns = new List<Dictionary<string, object>>();
        var nameObject = new Dictionary<string, object>();
        nameObject.Add("stationID", "1");
        nameObject.Add("stationName", "name");
        nameObject.Add("corporationID", "2");
        nameObject.Add("dockingCostPerVolume", "2.0");
        nameObject.Add("maxShipVolumeDockable", "2");
        nameObject.Add("officeRentalCost", "2");
        nameObject.Add("operationID", "2");
        nameObject.Add("reprocessingEfficiency", "2.0");
        nameObject.Add("reprocessingHangarFlag", "2");
        nameObject.Add("reprocessingStationsTake", "2.0");
        nameObject.Add("security", "2.0");
        nameObject.Add("regionID", "2");
        nameObject.Add("constellationID", "2");
        nameObject.Add("solarSystemID", "2");
        nameObject.Add("stationTypeID", "2");
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
        result.First().CorporationId.Should().Be(2);
        result.First().DockingCostPerVolume.Should().Be(2.0f);
        result.First().MaxShipVolumeDockable.Should().Be(2);
        result.First().OfficeRentalCost.Should().Be(2);
        result.First().OperationID.Should().Be(2);
        result.First().ReprocessingEfficiency.Should().Be(2.0f);
        result.First().ReprocessingHangarFlag.Should().Be(2);
        result.First().ReprocessingStationsTake.Should().Be(2.0f);
        result.First().Security.Should().Be(2.0);
        result.First().RegionID.Should().Be(2);
        result.First().CorporationId.Should().Be(2);
        result.First().SolarSystemID.Should().Be(2);
        result.First().TypeID.Should().Be(2);
    }

    [Fact]
    public async Task GetData_ReturnsArgumentNullException_WhenEmptyPath()
    {
        //arrange
        var _readerReturns = new List<Dictionary<string, object>>();
        var nameObject = new Dictionary<string, object>();
        _readerReturns.Add(nameObject);

        _readerMock
            .Setup(r => r.ReadYamlFileBSD(It.IsAny<string>()))
            .ReturnsAsync(_readerReturns);

        //act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _reader.GetData(""));
    }

    [Fact]
    public async Task GetData_ReturnsKeyNotFoundException_WhenEmptyDictionary()
    {
        //arrange
        var _readerReturns = new List<Dictionary<string, object>>();
        var nameObject = new Dictionary<string, object>();
        _readerReturns.Add(nameObject);

        _readerMock
            .Setup(r => r.ReadYamlFileBSD(It.IsAny<string>()))
            .ReturnsAsync(_readerReturns);

        //act & assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _reader.GetData("asdfa"));
    }
}
