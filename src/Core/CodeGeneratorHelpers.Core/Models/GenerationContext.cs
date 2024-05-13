using CodeGeneratorHelpers.Core.Internals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class GenerationContext
    {

        private readonly IFileService _fileService;
        private readonly GenerationState _generationState;

        private readonly SemaphoreSlim _filesReadLock = new(1, 1);

        public string RootFullPath => _generationState.RootFullPath;

        public string GenerationFullPath => _generationState.GenerationFullPath;

        private ConcurrentDictionary<string, CodeMetadata> FileMetaCache => _generationState._fileMetaCache;

        internal GenerationContext(IFileService fileService, GenerationState generationState)
        {
            _fileService = fileService;
            _generationState = generationState;
        }

        public Task<string> ReadTextInFileAsync(string filePath)
            => _fileService.ReadAllTextAsync(GetFullPath(filePath));

        public Task WriteAllTextToFileAsync(string filePath, string rawText)
            => _fileService.WriteAllTextAsync(GetFullPath(filePath), rawText);

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

        private async IAsyncEnumerable<IEnumerable<CodeMetadata>> InternalReadAllFilesMetaDataAsync(string folderPath = null,
                                                                                                    int maxDegreeOfParallelism = 10,
                                                                                                    Func<CodeMetadata, Task> action = null,
                                                                                                    bool useCache = true)
        {

            if (useCache)
                await _filesReadLock.WaitAsync();

            try
            {
                var path = GetFullPath(folderPath);

                var filePaths = _fileService.EnumerateFiles(path, "*")
                                             .Where(f => !f.StartsWith(GenerationFullPath, StringComparison.OrdinalIgnoreCase))
                                             .ToArray();

                var chunks = filePaths.Chunk(maxDegreeOfParallelism);

                foreach (var chunk in chunks)
                {
                    var allMetaData = new ConcurrentBag<CodeMetadata>();

                    var tasks = chunk.Select(f => Task.Run(async () =>
                    {
                        if (!(useCache && !FileMetaCache.TryGetValue(f, out var metaData)))
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
            finally
            {
                if (useCache)
                    _filesReadLock.Release();
            }
        }

        private string GetFullPath(string filePath)
            => filePath is null ? RootFullPath : _fileService.Combine(RootFullPath, filePath);

    }
}
