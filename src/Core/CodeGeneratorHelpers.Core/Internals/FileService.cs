using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Internals
{
    internal class FileService : IFileService
    {

        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public string Combine(string path1, string path2) => Path.Combine(path1, path2);

    }
}
