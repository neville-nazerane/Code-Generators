﻿using System.Runtime.CompilerServices;

namespace CodeGeneratorHelpers.Core.Internals
{
    public interface IFileService
    {
        string Combine(params string[] paths);
        string CombineToFullPath(params string[] paths);
        void CreateDirectory(string path);
        void DeleteDirectory(string path);
        void DeleteDirectory(string path, bool recursive);
        bool DirectoryExists(string path);
        IEnumerable<string> EnumerateFiles(string path, string searchPattern);
        string GetCurrentDirectory();
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string text);
    }
}