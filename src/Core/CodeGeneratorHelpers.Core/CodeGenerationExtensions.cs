using CodeGeneratorHelpers.Core.Internals;
using CodeGeneratorHelpers.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core
{
    public static class CodeGenerationExtensions
    {

        public static CodeMetadata GetCSharpCodeMetadata(this string rawCode, string sourceFilePath = null)
            => CodeUtility.GetCodeMetaData(rawCode, sourceFilePath);

    }
}
