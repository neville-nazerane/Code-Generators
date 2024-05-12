using CodeGeneratorHelpers.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core
{
    public partial class CodeGenerator
    {


        public async Task<CodeGenerator> RunAsync()
        {
            var fullTargetPath = GetFullTargetAppPath();

            var generationPath = _fileService.Combine(fullTargetPath, GenerationDestinationPath);

            // clean up generation path
            if (_fileService.DirectoryExists(generationPath) && ClearGenerationDestinationPath)
                _fileService.DeleteDirectory(generationPath, true);

            Directory.CreateDirectory(generationPath);

            var chunks = Processes.Chunk(MaxDegreeOfParallelism);

            foreach (var chunk in chunks)
            {
                var tasks = chunk.Select(async c =>
                {
                    var context = new GenerationContext
                    {
                        RootFullPath = fullTargetPath,
                        GenerationFullPath = generationPath
                    };
                    await c(null);
                }).ToArray();
                await Task.WhenAll(tasks);
            }

            return this;
        }

    }
}
