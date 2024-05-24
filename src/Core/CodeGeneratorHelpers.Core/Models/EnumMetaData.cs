using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class EnumMetadata : ICodeItem
    {

        public string Name { get; init; }

        public ClassMetadata ParentClass { get; init; }
        public string SourceFilePath { get; init; }
        public IEnumerable<string> Modifiers { get; internal set; }
        public IEnumerable<AttributeMetadata> Attributes { get; internal set; }
    }
}
