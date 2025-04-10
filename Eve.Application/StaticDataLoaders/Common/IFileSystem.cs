namespace Eve.Application.StaticDataLoaders.Common;
public interface IFileSystem
{
    bool Exists(string path);
    string ReadAllText(string path);
}

public class FileSystem : IFileSystem
{
    public bool Exists(string path) => File.Exists(path);

    public string ReadAllText(string path) => File.ReadAllText(path);
}