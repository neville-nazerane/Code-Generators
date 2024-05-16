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
using System.Xml.Linq;

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

            FillUpMetaData(root, metaData, sourceFilePath);

            return metaData;
        }

        #region private

        private static void FillUpMetaData(SyntaxNode rootNode,
                                           CodeMetadata metaModel,
                                           string sourceFilePath)
        {

            var parentClass = metaModel as ClassMetaData;

            metaModel.Classes = [];
            metaModel.Interfaces = [];
            metaModel.Enums = [];

            var classes = new List<ClassMetaData>();
            var interfaces = new List<InterfaceMetaData>();
            var enums = new List<EnumMetaData>();


            foreach (var node in rootNode.ChildNodes())
            {
                var type = node.GetType();
                switch (node)
                {
                    case NamespaceDeclarationSyntax _:
                        FillUpMetaData(node, metaModel, sourceFilePath);
                        break;
                    case ClassDeclarationSyntax classSyntax:
                        var classMeta = new ClassMetaData
                        {
                            ClassName = classSyntax.Identifier.Text,
                            ParentClass = parentClass,
                            SourceFilePath = sourceFilePath
                        };
                        FillUpMetaData(node, classMeta, sourceFilePath);
                        classes.Add(classMeta);
                        break;

                    case InterfaceDeclarationSyntax interfaceSyntax:
                        interfaces.Add(new()
                        {
                            InterfaceName = interfaceSyntax.Identifier.Text,
                            ParentClass = parentClass,
                            SourceFilePath = sourceFilePath
                        });
                        break;

                    case EnumDeclarationSyntax enumSyntax:
                        enums.Add(new()
                        {
                            EnumName = enumSyntax.Identifier.Text,
                            ParentClass = parentClass,
                            SourceFilePath = sourceFilePath
                        });
                        break;
                }
            }

            metaModel.Classes = metaModel.Classes.Union(classes).ToArray();
            metaModel.Interfaces = metaModel.Interfaces.Union(interfaces).ToArray();
            metaModel.Enums = metaModel.Enums.Union(enums).ToArray();
        }

        #endregion
    }
}
