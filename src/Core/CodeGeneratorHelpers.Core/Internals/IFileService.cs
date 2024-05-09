using System.Runtime.CompilerServices;

namespace CodeGeneratorHelpers.Core.Internals
{
    internal interface IFileService
    {
        string Combine(string path1, string path2);
        bool DirectoryExists(string path);
        string GetCurrentDirectory();
        IAsyncEnumerable<string> GetFileLinesAsync(string fullFilePath, [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}