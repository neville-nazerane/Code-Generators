using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class ParameterMetadata
    {

        public string Name { get; internal set; }
        public IEnumerable<AttributeMetadata> Attributes { get; internal set; }
        public TypeMetadata Type { get; internal set; }
    }
}
