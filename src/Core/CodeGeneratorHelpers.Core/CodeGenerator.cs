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
        private readonly GenerationState _state;


        /// <summary>
        /// Path of target app to generate code on
        /// </summary>
        public string TargetAppPath { get; }

        /// <summary>
        /// 
        /// Path within the target app to place generated files by default
        /// 
        /// Default: "Generated"
        /// </summary>
        public string GenerationDestinationPath { get; }

        /// <summary>
        /// Maximum number of processes that can to run parallel
        /// Defaults: 5
        /// </summary>
        public int MaxDegreeOfParallelism { get; }

        /// <summary>
        /// 
        /// If true, contents of GenerationDestinationPath would be emptied before generation processes begin
        /// Default: false
        /// 
        /// </summary>
        public bool ClearGenerationDestinationPath { get; }

        public string FullAppTargetPath => _state.RootFullPath;

        public string FullGenerationDestinationPath => _state.GenerationFullPath;

        internal CodeGenerator(IFileService fileService,
                               string targetAppPath,
                               string generationDestinationPath = "Generated",
                               int maxDegreeOfParallelism = 10,
                               bool clearGenerationDestinationPath = false)
        {
            _fileService = fileService;
            TargetAppPath = targetAppPath;
            GenerationDestinationPath = generationDestinationPath;
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
            ClearGenerationDestinationPath = clearGenerationDestinationPath;

            var targetPath = GetFullTargetAppPath();
            _state = new GenerationState
            {
                GenerationFullPath = _fileService.Combine(targetPath, GenerationDestinationPath),
                RootFullPath = targetPath
            };
        }

        public static CodeGenerator Create(string targetAppPath,
                                         string generationDestinationPath = "Generated",
                                         int maxDegreeOfParallelism = 10,
                                         bool clearGenerationDestinationPath = false) 
            => new(new FileService(),
                   targetAppPath,
                   generationDestinationPath,
                   maxDegreeOfParallelism,
                   clearGenerationDestinationPath);

        /// <summary>
        /// Get the full path of the target app based on the target app path set
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private string GetFullTargetAppPath()
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
