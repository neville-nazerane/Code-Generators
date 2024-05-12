using CodeGeneratorHelpers.Core.Internals;
using System;
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

        public string RootFullPath => _generationState.RootFullPath;

        public string GenerationFullPath => _generationState.GenerationFullPath;

        internal GenerationContext(IFileService fileService, GenerationState generationState)
        {
            _fileService = fileService;
            _generationState = generationState;
        }

        public Task<string> ReadTextInFileAsync(string filePath)
        {
            string fullFilePath = _fileService.Combine(RootFullPath, filePath);
            return _fileService.ReadAllTextAsync(fullFilePath);
        }

        public Task WriteAllTextToFileAsync(string filePath, string rawText)
        {
            string fullFilePath = _fileService.Combine(RootFullPath, filePath);
            return _fileService.WriteAllTextAsync(fullFilePath, rawText);
        }

        public async Task<CodeMetadata> ReadMetadataFromFileAsync(string filePath)
        {
            string fullFilePath = _fileService.Combine(RootFullPath, filePath);
            var text = await _fileService.ReadAllTextAsync(fullFilePath);
            return CodeUtility.GetCodeMetaData(text, filePath);
        }



    }
}
