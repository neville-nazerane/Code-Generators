using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class TypeMetadata
    {
        public string Name { get; internal set; }

        public bool IsNamedType { get; internal set; }

        public bool IsPredefinedType { get; internal set; }

        public string Keyword { get; internal set; }

        public string IdentifierName { get; internal set; }

        public IEnumerable<TypeMetadata> GenericArguments { get; internal set; }
        public bool IsGeneric { get; internal set; }
        public string FullName { get; internal set; }
    }
}
