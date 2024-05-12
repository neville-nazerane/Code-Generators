using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Internals
{
    internal class FileService : IFileService
    {

        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public void DeleteDirectory(string path) => Directory.Delete(path);
        public void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, true);

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public string Combine(string path1, string path2) => Path.Combine(path1, path2);

        public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);

        public Task WriteAllTextAsync(string path, string text) => File.WriteAllTextAsync(path, text);

    }
}
