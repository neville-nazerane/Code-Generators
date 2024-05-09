namespace CodeGeneratorHelpers.Core.Internals
{
    public interface IFileService
    {
        string Combine(string path1, string path2);
        bool DirectoryExists(string path);
        string GetCurrentDirectory();
    }
}