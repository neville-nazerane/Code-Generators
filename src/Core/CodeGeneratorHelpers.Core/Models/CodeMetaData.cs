using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class CodeMetadata
    {

        public IEnumerable<ClassMetaData> Classes { get; set; }
        public IEnumerable<InterfaceMetaData> Interfaces { get; internal set; }
        public IEnumerable<EnumMetaData> Enums { get; internal set; }
    }
}
