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

        public string Combine(params string[] paths) => Path.Combine(paths);

        public string CombineToFullPath(params string[] paths)
        {
            var res = new StringBuilder(Path.Combine(paths));

            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                res.Insert(0, Path.DirectorySeparatorChar);

            return res.ToString();
        }


        public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);

        public Task WriteAllTextAsync(string path, string text) => File.WriteAllTextAsync(path, text);

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
            => Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories);

    }
}
