using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public interface ICodeItem
    {

        public string Name { get; }

        public ClassMetadata ParentClass { get; }

        public string SourceFilePath { get; }

        public IEnumerable<string> Usings { get; }

    }
}
