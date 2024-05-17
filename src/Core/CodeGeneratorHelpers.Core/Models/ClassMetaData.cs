using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class ClassMetadata : CodeMetadata, ICodeItem
    {
        public string ClassName { get; init; }

        public ClassMetadata ParentClass { get; init; }

        public IEnumerable<PropertyMetadata> Properties { get; internal set; }

    }
}
