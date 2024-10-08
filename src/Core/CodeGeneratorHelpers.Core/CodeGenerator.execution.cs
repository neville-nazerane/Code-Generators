﻿using CodeGeneratorHelpers.Core.Internals;
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

        public Task<string> ReadTextInFileAsync(string filePath)
            => _fileService.ReadAllTextAsync(GetFullPath(filePath));

        public Task WriteAllTextToFileAsync(string filePath, string rawText)
            => _fileService.WriteAllTextAsync(GetFullPath(filePath, FullGenerationDestinationPath), rawText);

        public async Task<CodeMetadata> GetFileMetadataAsync(string filePath)
        {
            string fullFilePath = GetFullPath(filePath);
            var text = await _fileService.ReadAllTextAsync(fullFilePath);
            var metadata = CodeUtility.GetCodeMetadata(text, filePath);
            return metadata;
        }

        public async Task ExecuteOnEachFileAsync(Func<CodeMetadata, Task> execution,
                                                 string folderPath = null,
                                                 string filePattern = "*.cs",
                                                 int maxDegreeOfParallelism = 10)
        {
            var items = InternalReadAllFilesMetaDataAsync(folderPath, filePattern, maxDegreeOfParallelism, execution);
            await foreach (var _ in items) ;
        }

        public IAsyncEnumerable<IEnumerable<CodeMetadata>> GetAllFilesMetadataInBatchesAsync(string folderPath = null,
                                                                                             string filePattern = "*.cs",
                                                                                             int batchSize = 20)
            => InternalReadAllFilesMetaDataAsync(folderPath, filePattern, batchSize, null);


        public async IAsyncEnumerable<CodeMetadata> GetAllFilesMetadataAsAsyncEnumerable(string folderPath = null,
                                                                                      string filePattern = "*.cs")
        {
            var items = InternalReadAllFilesMetaDataAsync(folderPath, filePattern, 1, null);
            await foreach (var item in items)
                yield return item.Single();
        }

        public async Task<IEnumerable<CodeMetadata>> GetAllFilesMetadataAsync(string folderPath = null,
                                                                         string filePattern = "*.cs",
                                                                         int maxDegreeOfParallelism = 20)
        {
            var items = InternalReadAllFilesMetaDataAsync(folderPath, filePattern, maxDegreeOfParallelism, null);
            var res = new List<CodeMetadata>();

            await foreach (var item in items)
                res.AddRange(item);

            return res;
        }

        #region Private
        
        private async IAsyncEnumerable<IEnumerable<CodeMetadata>> InternalReadAllFilesMetaDataAsync(string folderPath = null,
                                                                                                    string filePattern = "*.cs",
                                                                                                    int maxDegreeOfParallelism = 10,
                                                                                                    Func<CodeMetadata, Task> action = null)
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
                    var metaData = await GetFileMetadataAsync(f);

                    allMetaData.Add(metaData);
                    if (action is not null)
                        await action(metaData);
                }));

                await Task.WhenAll(tasks);

                yield return allMetaData.ToArray();
            }

        }


        private string GetFullPath(string filePath, string basePath = null)
        {
            basePath ??= FullAppTargetPath;
            return filePath is null ? basePath : _fileService.CombineToFullPath(basePath, filePath);
        }

        #endregion
    }
}
