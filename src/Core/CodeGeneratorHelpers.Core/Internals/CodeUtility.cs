using CodeGeneratorHelpers.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        internal static CodeMetadata GetCodeMetaData(string rawCode)
        {
            var res = new CodeMetadata();

            var tree = CSharpSyntaxTree.ParseText(rawCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            FillUpMetaData(root, res);

            return res;
        }

        private static void FillUpMetaData(SyntaxNode rootNode, CodeMetadata metaModel)
        {
            var classes = new List<ClassMetaData>();
            var interfaces = new List<InterfaceMetaData>();
            var enums = new List<EnumMetaData>();

            foreach (var node in rootNode.ChildNodes())
            {
                switch (node)
                {
                    case ClassDeclarationSyntax classSyntax:
                        var classMeta = new ClassMetaData
                        {
                            ClassName = classSyntax.Identifier.Text
                        };
                        FillUpMetaData(node, classMeta);
                        classes.Add(classMeta);
                        break;

                    case InterfaceDeclarationSyntax interfaceSyntax:
                        interfaces.Add(new()
                        {
                            InterfaceName = interfaceSyntax.Identifier.Text
                        });
                        break;

                    case EnumDeclarationSyntax enumSyntax:
                        enums.Add(new()
                        {
                            EnumName = enumSyntax.Identifier.Text
                        });
                        break;
                }
            }

            metaModel.Classes = classes;
            metaModel.Interfaces = interfaces;
            metaModel.Enums = enums;
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
