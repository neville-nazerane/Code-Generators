using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class ProcessForAllFilesRequest
    {

        public int MaxDegreeOfParallelism { get; set; }

        public string FolderPath { get; set; }

        public string FileSearchPattern { get; set; }

        public IEnumerable<Func<GenerationContext, Task>> Processes { get; set; } = [];

    }
}
