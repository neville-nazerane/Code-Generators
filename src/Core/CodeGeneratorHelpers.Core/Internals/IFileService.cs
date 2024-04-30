namespace CodeGeneratorHelpers.Core.Internals
{
    public interface IFileService
    {
        bool DirectoryExists(string path);
        string GetCurrentDirectory();
    }
}