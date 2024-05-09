using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Internals
{
    public class FileService : IFileService
    {

        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public string Combine(string path1, string path2) => Path.Combine(path1, path2);

        public async IAsyncEnumerable<string> GetFileLinesAsync(string fullFilePath, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            using var streamReader = new StreamReader(fullFilePath);
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (line is not null)
                    yield return line;
            }
        }

    }
}
