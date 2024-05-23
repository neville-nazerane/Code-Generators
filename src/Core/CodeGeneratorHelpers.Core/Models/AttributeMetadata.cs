using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class AttributeMetadata
    {
        public string Name { get; internal set; }

        public IEnumerable<string> ArgumentValues { get; internal set; }

    }
}
