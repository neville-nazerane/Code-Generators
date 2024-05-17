using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class EnumMetadata : ICodeItem
    {

        public string EnumName { get; init; }

        public ClassMetadata ParentClass { get; init; }
        public string SourceFilePath { get; init; }

    }
}
