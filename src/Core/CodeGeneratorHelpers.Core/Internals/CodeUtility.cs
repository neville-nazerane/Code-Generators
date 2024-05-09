﻿using CodeGeneratorHelpers.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Internals
{
    internal partial class CodeUtility
    {

        [GeneratedRegex(@"^(?<accessibility>public|private|internal|protected)?\s*(?<modifier>static|abstract)?\s*class\s+(?<className>\w+)(?<openBracket>\s*{\s*)?(?<closeBracket>\s*}\s*)?$", RegexOptions.Compiled)]
        private static partial Regex ClassMatch();

        internal static async Task<CodeMetadata> GetCodeMetaDataAsync(IAsyncEnumerable<string> lines, CancellationToken cancellationToken = default)
        {
            var result = new CodeMetadata();
            await foreach (var line in lines)
            {
                var pattern = ClassMatch();
                var match = pattern.Match(line);

                if (match.Success)
                {
                    var accessibility = match.Groups["accessibility"].Value.Trim();
                    var modifier = match.Groups["modifier"].Value.Trim();
                    var className = match.Groups["className"].Value.Trim();
                    var open = match.Groups["openBracket"].Success;
                    
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            return result;
        }

        internal static Task<CodeMetadata> GetCodeMetaDataAsync(string rawString, CancellationToken cancellationToken = default)
        {
            var lines = AsAsyncEnumerableLines(rawString, cancellationToken);
            return GetCodeMetaDataAsync(lines, cancellationToken);
        }

        static async IAsyncEnumerable<string> AsAsyncEnumerableLines(string rawString, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            using var stringReader = new StringReader(rawString);
            string line;
            while ((line = await stringReader.ReadLineAsync(cancellationToken)) is not null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return line;
            }
        }


    }
}
