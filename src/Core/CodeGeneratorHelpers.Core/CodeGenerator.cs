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

    public partial class CodeGenerator
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
        public string GenerationDestinationPath { get; set; } = "Generated";

        /// <summary>
        /// Maximum number of processes that can to run parallel
        /// Defaults: 5
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = 5;

        /// <summary>
        /// 
        /// If true, contents of GenerationDestinationPath would be emptied before generation processes begin
        /// Default: false
        /// 
        /// </summary>
        public bool ClearGenerationDestinationPath { get; set; }

        /// <summary>
        /// Processes to execute
        /// </summary>
        public List<Func<GenerationContext, Task>> Processes { get; }
        
        /// <summary>
        /// Key value pairs for processes to run on all files of folders
        /// Key: folder name or pattern
        /// Value: Process to be executed for each file in folder
        /// </summary>
        public List<ProcessForAllFilesRequest> ProcessesForFilesInFolder { get; }

        internal CodeGenerator(IFileService fileService)
        {
            _fileService = fileService;
            Processes = [];
            ProcessesForFilesInFolder = [];
        }

        public CodeGenerator() : this(new FileService()) { }

        /// <summary>
        /// 
        /// Empty contents of GenerationDestinationPath before generation processes begin
        /// Default: false
        /// 
        /// </summary>
        /// <param name="clear"></param>
        /// <returns></returns>
        public CodeGenerator EnsureGenerationDestinationPathCleared(bool clear = true)
        {
            ClearGenerationDestinationPath = clear;
            return this;
        }

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
            GenerationDestinationPath = path;
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

        //public CodeGenerator AddProcessForEachAllFiles()
        //{

        //}

        /// <summary>
        /// Get the full path of the target app based on the target app path set
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public string GetFullTargetAppPath()
        {
            char[] dirSeparators = ['/', '\\'];

            var cleanedAppPath = Path.Combine(TargetAppPath.Split(dirSeparators));
            var dirs = _fileService.GetCurrentDirectory().Split(dirSeparators);

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
