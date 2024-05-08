﻿using CodeGeneratorHelpers.Core.Internals;
using CodeGeneratorHelpers.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("CodeGeneratorHelpers.Core.Tests")]

namespace CodeGeneratorHelpers.Core
{

    public class CodeGenerator
    {

        private readonly IFileService _fileService;

        /// <summary>
        /// Path of target app to generate code on
        /// </summary>
        public string TargetAppPath { get; set; }

        /// <summary>
        /// 
        /// Path within the target app to place generated files by default
        /// 
        /// Default: "Generated"
        /// </summary>
        public string GeneratedFilesPath { get; set; } = "Generated";

        public List<Func<GenerationContext, Task>> Processes { get; }

        internal CodeGenerator(IFileService fileService)
        {
            _fileService = fileService;
            Processes = [];
        }

        public CodeGenerator() : this(new FileService()) { }

        /// <summary>
        /// Path of target app to generate code on
        /// </summary>
        public CodeGenerator SetTargetAppPath(string path)
        {
            TargetAppPath = path;
            return this;
        }

        /// <summary>
        /// 
        /// Path within the target app to place generated files by default
        /// 
        /// Default: "Generated"
        /// </summary>
        public CodeGenerator SetGeneratedFilesPath(string path)
        {
            GeneratedFilesPath = path;
            return this;
        }

        /// <summary>
        /// Add a process to be executed during generation
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public CodeGenerator AddProcess(Func<GenerationContext, Task> process)
        {
            Processes.Add(process);
            return this;
        }

        /// <summary>
        /// Get the full path of the target app based on the target app path set
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public string GetFullTargetAppPath()
        {
            char[] dirSeperators = ['/', '\\'];

            var cleanedAppPath = Path.Combine(TargetAppPath.Split(dirSeperators));
            var dirs = _fileService.GetCurrentDirectory().Split(dirSeperators);

            var triedPaths = new List<string>();

            for (int i = dirs.Length - 1; i >= 0; i--)
            {
                string path = Path.Combine(dirs.Take(i).ToArray());
                if (path.EndsWith(cleanedAppPath) && _fileService.DirectoryExists(path))
                    return path;
                triedPaths.Add(path);
                path = Path.Combine(path, cleanedAppPath);
                if (_fileService.DirectoryExists(path))
                    return path;
                triedPaths.Add(path);
            }

            throw new DirectoryNotFoundException(
                $"Failed to find app path. Tried {string.Join('\n', triedPaths.Select(t => $"'{t}'").ToArray())}");
        }

    }
}
