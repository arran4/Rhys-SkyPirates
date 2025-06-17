public interface IFileLoader
{
    string ReadAllText(string path);
    void WriteAllText(string path, string content);
}
