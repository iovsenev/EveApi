
namespace Eve.Application.StaticDataLoaders.Common;

public interface IFileReader
{
    Task<List<Dictionary<string, object>>> ReadYamlFileBSD(string path);
    Task<Dictionary<int, Dictionary<string, object>>> ReadYamlFileFSD(string path);
    Task<Dictionary<string, object>> ReadYamlFileUniverse(string path);
}