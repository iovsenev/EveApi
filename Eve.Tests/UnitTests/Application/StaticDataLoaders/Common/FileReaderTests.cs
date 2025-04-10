using Eve.Application.StaticDataLoaders.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eve.Tests.UnitTests.Application.StaticDataLoaders.Common;
public class FileReaderTests
{
    private readonly FileReader _reader;
    private readonly Mock<IFileSystem> _fileSystem;
    private readonly Mock<ILogger<FileReader>> _logger;

    public FileReaderTests()
    {
        _fileSystem = new();
        _logger = new();

        _reader = new(_fileSystem.Object, _logger.Object);
    }

    [Fact]
    public async Task ReadYamlFileFSD_ReturnsSuccess()
    {
        //arrange
        _fileSystem
            .Setup(fs => fs.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileSystem
            .Setup(fs => fs.ReadAllText(It.IsAny<string>()))
            .Returns(GetFSDEntity());

        //act
        var result = await _reader.ReadYamlFileFSD("qer");

        //assert
        result.Count().Should().Be(1);
    }

    [Fact]
    public async Task ReadYamlFileAllMethods_ReturnsError_WhenNotExistFile()
    {
        //arrange
        _fileSystem
            .Setup(fs => fs.Exists(It.IsAny<string>()))
            .Returns(false);
        _fileSystem
            .Setup(fs => fs.ReadAllText(It.IsAny<string>()))
            .Returns("");

        //act & assert
        await Assert.ThrowsAsync<FileLoadException>(() => _reader.ReadYamlFileFSD("1"));
        await Assert.ThrowsAsync<FileLoadException>(() => _reader.ReadYamlFileBSD("1"));
        await Assert.ThrowsAsync<FileLoadException>(() => _reader.ReadYamlFileUniverse("1"));
    }

    [Fact]
    public async Task ReadYamlFileBSD_ReturnsSuccess()
    {
        //arrange
        _fileSystem
            .Setup(fs => fs.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileSystem
            .Setup(fs => fs.ReadAllText(It.IsAny<string>()))
            .Returns(GetBSDEntity());

        //act
        var result = await _reader.ReadYamlFileBSD("qer");

        //assert
        result.Count().Should().Be(3);
    }

    [Fact]
    public async Task ReadYamlFileUniverse_ReturnsSuccess()
    {
        //arrange
        _fileSystem
            .Setup(fs => fs.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileSystem
            .Setup(fs => fs.ReadAllText(It.IsAny<string>()))
            .Returns(GetUniverseEntity());

        //act
        var result = await _reader.ReadYamlFileUniverse("qer");

        //assert
        result.Count().Should().Be(6);
    }

    private string GetFSDEntity()
    {
        return $@"6: 
  description:
    de: Dieser Hauptreihenstern gehört einer Klasse an, die sich oft durch eine gelbe
      oder gelblich-orange Farbe auszeichnet und über den gewaltigen Wasserstofffusionsprozess
      im Herzen des Sterns eine enorme Menge Energie erzeugt und ausstrahlt. Im Orbit
      dieser Planeten können sowohl terrestrische Planeten als auch Gasriesen in unterschiedlicher
      Anzahl gefunden werden, von denen sich einer oder mehrere in der bewohnbaren
      Zone befindet.
    en: ""A main-sequence stellar body of a class that is often yellow or yellow-orange
      in hue, generating and emitting energy from the vast hydrogen fusion process
      within the heart of the star.\r\n\r\nVarious numbers of planets of the terrestrial
      and gas giant types are found around these stars and the habitable zones often
      contain one or more planets.""
    es: 'Un cuerpo estelar de secuencia principal de una clase con un tono a menudo
      amarillento, o entre amarillento y anaranjado, que genera y emite energía a
      raíz del vasto proceso de fusión de hidrógeno que se produce en su corazón.


      A su alrededor suele haber planetas y gigantes gaseosos varios, y las zonas
      habitables por lo general contienen uno o más planetas.'
    fr: Communément jaunes ou jaune orangé, ces étoiles de la séquence principale
      génèrent et émettent de l'énergie par fusion des noyaux d'hydrogène emprisonnés
      dans leur cœur. Leur écosphère stellaire engendre la formation de planètes géantes
      de type terrestre ou gazeux, dont quelques spécimens gravitent souvent en zone
      habitable.
    ja: ""黄色や黄橙色をしていることが多いクラスの主系列恒星体で、星の中心部の広大な水素核融合過程でエネルギーを発生、放出している。\r\n\nこれらの星の周りには、地球型やガス巨人型の惑星が多数存在し、居住可能区域には1つ以上の惑星が含まれていることが多い。""
    ko: 오렌지색 항성은 내핵의 수소융합반응을 바탕으로 막대한 에너지를 방출합니다. 항성 주변에 지상형 행성을 비롯한 가스 행성이 주로 발견되며
      일반적으로 한 개 이상의 지구형 행성 또한 관측됩니다.
    ru: Небесное тело главной последовательности, принадлежащее к классу звёзд, которые
      имеют жёлтый или жёлто-оранжевый оттенок, а также производят и излучают огромное
      количество энергии, образующейся в результате синтеза ядер гелия из водорода
      в их ядре. Рядом с такими звёздами встречается большое количество планет земного
      типа и газовых гигантов, а некоторые из этих планет находятся в потенциально
      обитаемой зоне.
    zh: '一种主星序星体，通常是黄色或黄橙色的色调，通过巨大的氢融合过程从恒星内产生和发射能量。




      这些恒星周围发现了大量陆地和气体巨星，而居住区通常包含一个或多个行星。'
  graphicID: 21457
  groupID: 6
  mass: 1.0e+18
  name:
    de: Sun G5 (Yellow)
    en: Sun G5 (Yellow)
    es: Sol G5 (amarillo)
    fr: Soleil G5 (jaune)
    ja: 恒星G5（イエロー）
    ko: G5 항성 (황색)
    ru: Sun G5 (Yellow)
    zh: 恒星G5(黄色)
  portionSize: 1
  published: false
  radius: 10000.0
  volume: 1.0";
    }
    private string GetBSDEntity()
    {
        return @"- groupID: 19
  itemID: 500001
  itemName: Caldari State
- groupID: 19
  itemID: 500002
  itemName: Minmatar Republic
- groupID: 19
  itemID: 500003
  itemName: Amarr Empire";
    }

    private string GetUniverseEntity()
    {
        return @"center:
- -3.224627916385381e+17
- 7249403267726650.0
- -3.851040467060743e+16
constellationID: 20000619
max:
- -3.1564711659598694e+17
- 1.7018359145023234e+16
- 4.55221796593872e+16
min:
- -3.292784666810893e+17
- -2519552609569933.5
- 3.1498629681827664e+16
nameID: 268466
radius: 9768955877296584.0";
    }
}
