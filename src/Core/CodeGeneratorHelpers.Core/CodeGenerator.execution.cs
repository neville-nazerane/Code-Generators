using CodeGeneratorHelpers.Core.Internals;
using CodeGeneratorHelpers.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core
{
    public partial class CodeGenerator
    {

        private ConcurrentDictionary<string, CodeMetadata> FileMetaCache => _state._fileMetaCache;

        public Task<string> ReadTextInFileAsync(string filePath)
            => _fileService.ReadAllTextAsync(GetFullPath(filePath));

        public Task WriteAllTextToFileAsync(string filePath, string rawText)
            => _fileService.WriteAllTextAsync(GetFullPath(filePath, FullGenerationDestinationPath), rawText);

        public async Task<CodeMetadata> ReadMetadataFromFileAsync(string filePath, bool useCache = true)
        {
            if (!(useCache && FileMetaCache.TryGetValue(filePath, out var metadata)))
            {
                string fullFilePath = GetFullPath(filePath);
                var text = await _fileService.ReadAllTextAsync(fullFilePath);
                metadata = CodeUtility.GetCodeMetaData(text, filePath);
            }
            return metadata;
        }

        private string GetFullPath(string filePath, string basePath = null)
        {
            if (basePath is null) basePath = FullAppTargetPath;
            return filePath is null ? basePath : _fileService.Combine(basePath, filePath);
        }

        public async Task ExecuteOnEachFileAsync(string folderPath = null,
                                                 string filePattern = "**/*.cs",
                                                 int maxDegreeOfParallelism = 10,
                                                 Func<CodeMetadata, Task> execution = null,
                                                 bool useCache = true)
        {
            var items = InternalReadAllFilesMetaDataAsync(folderPath, filePattern, maxDegreeOfParallelism, execution, useCache);
            await foreach (var _ in items);
        }

        public IAsyncEnumerable<IEnumerable<CodeMetadata>> GetFilesMetaInBatchesAsync(string folderPath = null,
                                                                                      string filePattern = "**/*.cs",
                                                                                      int batchSize = 10,
                                                                                      bool useCache = true) 
            => InternalReadAllFilesMetaDataAsync(folderPath, filePattern, batchSize, null, useCache);


        public async IAsyncEnumerable<CodeMetadata> GetFilesMetaAsAsyncEnumerable(string folderPath = null,
                                                                                      string filePattern = "**/*.cs",
                                                                                      bool useCache = true)
        {
            var items = InternalReadAllFilesMetaDataAsync(folderPath, filePattern, 1, null, useCache);
            await foreach (var item in items)
                yield return item.Single();
        }

        public async Task<IEnumerable<CodeMetadata>> GetAllFileMetaAsync(string folderPath = null,
                                                                         string filePattern = "**/*.cs",
                                                                         int maxDegreeOfParallelism = 10,
                                                                        bool useCache = true)
        {
            var items = InternalReadAllFilesMetaDataAsync(folderPath, filePattern, maxDegreeOfParallelism, null, useCache);
            var res = new List<CodeMetadata>();

            await foreach (var item in items)
                res.AddRange(item);

            return res;
        }

        private async IAsyncEnumerable<IEnumerable<CodeMetadata>> InternalReadAllFilesMetaDataAsync(string folderPath = null,
                                                                                                    string filePattern = "**/*.cs",
                                                                                                    int maxDegreeOfParallelism = 10,
                                                                                                    Func<CodeMetadata, Task> action = null,
                                                                                                    bool useCache = true)
        {

            var path = GetFullPath(folderPath);

            var filePaths = _fileService.EnumerateFiles(path, filePattern)
                                         .Where(f => !f.StartsWith(GenerationDestinationPath, StringComparison.OrdinalIgnoreCase))
                                         .ToArray();

            var chunks = filePaths.Chunk(maxDegreeOfParallelism);

            foreach (var chunk in chunks)
            {
                var allMetaData = new ConcurrentBag<CodeMetadata>();

                var tasks = chunk.Select(f => Task.Run(async () =>
                {
                    if (!(useCache && FileMetaCache.TryGetValue(f, out var metaData)))
                        metaData = await ReadMetadataFromFileAsync(f, false);

                    allMetaData.Add(metaData);
                    if (action is not null)
                        await action(metaData);
                }));

                await Task.WhenAll(tasks);

                if (useCache)
                    foreach (var metaData in allMetaData)
                        if (!FileMetaCache.ContainsKey(metaData.SourceFilePath))
                            FileMetaCache[metaData.SourceFilePath] = metaData;

                yield return allMetaData.ToArray();
            }

        }

        //public async Task<CodeGenerator> RunAsync()
        //{
        //    var fullTargetPath = GetFullTargetAppPath();

        //    // TODO handle /\
        //    var generationPath = _fileService.Combine(fullTargetPath, GenerationDestinationPath);

        //    // clean up generation path
        //    if (_fileService.DirectoryExists(generationPath) && ClearGenerationDestinationPath)
        //        _fileService.DeleteDirectory(generationPath, true);

        //    _fileService.CreateDirectory(generationPath);

        //    var chunks = Processes.Chunk(MaxDegreeOfParallelism);

        //    var state = new GenerationState
        //    {
        //        RootFullPath = fullTargetPath,
        //        GenerationFullPath = generationPath
        //    };

        //    foreach (var chunk in chunks)
        //    {
        //        var tasks = chunk.Select(async c =>
        //        {
        //            var context = new GenerationContext(null, null);
        //            await c(null);
        //        }).ToArray();
        //        await Task.WhenAll(tasks);
        //    }

        //    return this;
        //}

    }
}
