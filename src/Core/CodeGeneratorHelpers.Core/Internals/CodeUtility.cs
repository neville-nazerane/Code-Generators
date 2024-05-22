using CodeGeneratorHelpers.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

            var parentClass = metaModel as ClassMetadata;

            metaModel.Classes = [];
            metaModel.Interfaces = [];
            metaModel.Enums = [];

            var classes = new List<ClassMetadata>();
            var interfaces = new List<InterfaceMetadata>();
            var enums = new List<EnumMetadata>();


            foreach (var node in rootNode.ChildNodes())
            {
                switch (node)
                {
                    case NamespaceDeclarationSyntax _:
                        FillUpMetaData(node, metaModel, sourceFilePath);
                        break;
                    case ClassDeclarationSyntax classSyntax:
                        ClassMetadata classMeta = GetMetadata(classSyntax, sourceFilePath, parentClass, node);
                        classes.Add(classMeta);
                        break;

                    case InterfaceDeclarationSyntax interfaceSyntax:
                        interfaces.Add(new()
                        {
                            Name = interfaceSyntax.Identifier.Text,
                            ParentClass = parentClass,
                            SourceFilePath = sourceFilePath
                        });
                        break;

                    case EnumDeclarationSyntax enumSyntax:
                        enums.Add(new()
                        {
                            Name = enumSyntax.Identifier.Text,
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

        private static ClassMetadata GetMetadata(ClassDeclarationSyntax classSyntax,
                                                 string sourceFilePath,
                                                 ClassMetadata parentClass,
                                                 SyntaxNode node)
        {

            var classMeta = new ClassMetadata
            {
                Name = classSyntax.Identifier.Text,
                ParentClass = parentClass,
                SourceFilePath = sourceFilePath,
            };


            var properties = new List<PropertyMetadata>();
            var fields = new List<FieldMetadata>();

            foreach (var member in classSyntax.Members)
            {
                switch (member)
                {
                    case PropertyDeclarationSyntax propertyDeclarationSyntax:
                        properties.Add(GetMetadata(propertyDeclarationSyntax, sourceFilePath, classMeta));
                        break;
                    case FieldDeclarationSyntax fieldDeclarationSyntax:
                        fields.AddRange(GetMetadata(fieldDeclarationSyntax, sourceFilePath, classMeta));
                        break;
                }
            }

            classMeta.Properties = [.. properties];
            classMeta.Fields = [.. fields];


            foreach (var p in properties)
                p.ParentClass = classMeta;

            FillUpMetaData(node, classMeta, sourceFilePath);
            return classMeta;
        }

        private static PropertyMetadata GetMetadata(PropertyDeclarationSyntax syntax,
                                                    string sourceFilePath,
                                                    ClassMetadata ParentClass)
            => new()
            {
                Name = syntax.Identifier.Text,
                ParentClass = ParentClass,
                SourceFilePath = sourceFilePath
            };

        private static IEnumerable<FieldMetadata> GetMetadata(FieldDeclarationSyntax syntax,
                                                              string sourceFilePath,
                                                              ClassMetadata ParentClass)
        {
            var typeMeta = GetMetadata(syntax.Declaration.Type);

            return syntax.Declaration
                             .Variables
                             .Select(v => new FieldMetadata()
                             {
                                 Name = v.Identifier.Text,
                                 ParentClass = ParentClass,
                                 SourceFilePath = sourceFilePath,
                                 Type = typeMeta
                             }).ToImmutableArray();
        }

        private static TypeMetadata GetMetadata(TypeSyntax type)
        {
            var res = new TypeMetadata();
            switch (type)
            {
                case GenericNameSyntax genericName:
                    res.IsGeneric = true;
                    res.GenericArguments = genericName.TypeArgumentList.Arguments.Select(GetMetadata);
                    res.IdentifierName = genericName.Identifier.Text;
                    break;
                case PredefinedTypeSyntax predefinedType:
                    res.IsPredefinedType = true;
                    res.Keyword = predefinedType.Keyword.Text;
                    break;
                case IdentifierNameSyntax identifierName:
                    res.IdentifierName = identifierName.Identifier.Text;
                    break;
            }

            return res;
        }

        #endregion
    }
}
