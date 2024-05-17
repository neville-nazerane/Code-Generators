using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class PropertyMetadata : ICodeItem
    {

        public string PropertyName { get; set; }

        public ClassMetadata ParentClass { get; init; }
        public string SourceFilePath { get; init; }

    }
}
