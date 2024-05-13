using CodeGeneratorHelpers.Core.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class FileGenerationContext : GenerationContext
    {

        public string FileName { get; init; }

        public CodeMetadata CodeMetadata { get; init; }

        internal FileGenerationContext(IFileService fileService, GenerationState generationState) : base(fileService, generationState)
        {
            
        }

    }
}
