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

        internal static CodeMetadata GetCodeMetaData(string rawCode, string sourceFilePath = null)
        {
            var metaData = new CodeMetadata
            {
                SourceFilePath = sourceFilePath
            };

            var tree = CSharpSyntaxTree.ParseText(rawCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            FillUpMetaData(root, metaData);

            return metaData;
        }

        #region private

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

        #endregion
    }
}
