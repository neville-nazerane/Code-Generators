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

        public string RootFullPath { get; set; }

        public string GenerationFullPath { get; set; }

        internal GenerationContext(IFileService fileService)
        {
            _fileService = fileService;
        }

        public GenerationContext() : this(new FileService())
        {
            
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

    }
}
