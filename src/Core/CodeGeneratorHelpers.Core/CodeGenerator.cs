using CodeGeneratorHelpers.Core.Internals;
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

        /// <summary>
        /// Maximum number of processes that can to run parallel
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = 5;

        /// <summary>
        /// Processes to execute
        /// </summary>
        public List<Func<GenerationContext, Task>> Processes { get; }
        
        /// <summary>
        /// Key value pairs for processes to run on all files of folders
        /// Key: folder name or pattern
        /// Value: Process to be executed for each file in folder
        /// </summary>
        public List<KeyValuePair<string, Func<GenerationFileContext, Task>>> ProcessesForFilesInFolder { get; }

        internal CodeGenerator(IFileService fileService)
        {
            _fileService = fileService;
            Processes = [];
            ProcessesForFilesInFolder = [];
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
        /// Set maximum number of processes that can to run parallel
        /// </summary>
        /// <param name="maxDegreeOfParallelism"></param>
        /// <returns></returns>
        public CodeGenerator SetMaxDegreeOfParallelism(int maxDegreeOfParallelism)
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
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
        /// Add a process to run on each file on a folder
        /// </summary>
        /// <param name="pattern">Pattern of folder. For instance: '*ViewModels'</param>
        /// <param name="process">Process to execute on each file in folder(s) that match the key</param>
        /// <returns></returns>
        public CodeGenerator AddProcessForEachFileInFolder(string pattern, Func<GenerationContext, Task> process)
        {
            ProcessesForFilesInFolder.Add(new(pattern, process));
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



        //public async Task<CodeGenerator> RunAsync()
        //{
        //    Parallel.ForEachAsync(Processes)

        //    return this;
        //}

    }
}
