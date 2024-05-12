using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class EnumMetaData : ICodeItem
    {

        public string EnumName { get; init; }

        public ClassMetaData ParentClass { get; init; }
        public string SourceFilePath { get; init; }

    }
}
