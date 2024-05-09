using CodeGeneratorHelpers.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Internals
{
    internal class CodeUtility
    {

        internal static async Task<CodeMetadata> GetCodeMetaDataAsync(IAsyncEnumerable<string> lines, CancellationToken cancellationToken = default)
        {
            var result = new CodeMetadata();
            await foreach (var line in lines)
            {

                cancellationToken.ThrowIfCancellationRequested();
            }

            return result;
        }

        internal static Task<CodeMetadata> GetCodeMetaDataAsync(string rawString, CancellationToken cancellationToken)
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
