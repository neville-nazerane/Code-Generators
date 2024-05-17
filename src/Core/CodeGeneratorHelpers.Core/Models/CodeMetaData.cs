using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class CodeMetadata
    {

        public string SourceFilePath { get; init; }

        public IEnumerable<ClassMetadata> Classes { get; internal set; }
        public IEnumerable<InterfaceMetadata> Interfaces { get; internal set; }
        public IEnumerable<EnumMetadata> Enums { get; internal set; }

    }
}
