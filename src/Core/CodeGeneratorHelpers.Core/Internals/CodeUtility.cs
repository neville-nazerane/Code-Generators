using CodeGeneratorHelpers.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGeneratorHelpers.Core.Internals
{
    internal partial class CodeUtility
    {

        internal static CodeMetadata GetCodeMetadata(string rawCode, string sourceFilePath = null)
        {
            var metaData = new CodeMetadata
            {
                SourceFilePath = sourceFilePath
            };

            var tree = CSharpSyntaxTree.ParseText(rawCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            FillUpMetaData(root, metaData, null, null, sourceFilePath);

            return metaData;
        }

        #region private

        private static void FillUpMetaData(SyntaxNode rootNode,
                                           CodeMetadata metaModel,
                                           IEnumerable<string> usings,
                                           string namespaceName,
                                           string sourceFilePath)
        {

            var parentClass = metaModel as ClassMetadata;

            metaModel.Classes = [];
            metaModel.Interfaces = [];
            metaModel.Enums = [];

            var classes = new List<ClassMetadata>();
            var interfaces = new List<InterfaceMetadata>();
            var enums = new List<EnumMetadata>();

            usings ??= rootNode.DescendantNodes()
                                .OfType<UsingDirectiveSyntax>()
                                .Select(usingDirective => usingDirective.ToString())
                                .ToImmutableArray();

            foreach (var node in rootNode.ChildNodes())
            {
                switch (node)
                {
                    case FileScopedNamespaceDeclarationSyntax fileScopedNamespaceSyntax:
                        FillUpMetaData(node, metaModel, usings, fileScopedNamespaceSyntax.Name.ToString(), sourceFilePath);
                        break;
                    case NamespaceDeclarationSyntax namespaceSyntax:
                        FillUpMetaData(node, metaModel, usings, namespaceSyntax.Name.ToString(), sourceFilePath);
                        break;
                    case ClassDeclarationSyntax classSyntax:
                        classes.Add(GetMetadata(classSyntax, sourceFilePath, usings, namespaceName, parentClass, node));
                        break;

                    case InterfaceDeclarationSyntax interfaceSyntax:
                        interfaces.Add(GetMetadata(interfaceSyntax, sourceFilePath, usings, namespaceName, parentClass));
                        break;

                    case EnumDeclarationSyntax enumSyntax:
                        enums.Add(GetMetaData(enumSyntax, sourceFilePath, usings, namespaceName, parentClass));
                        break;
                }
            }

            metaModel.Classes = metaModel.Classes.Union(classes).ToArray();
            metaModel.Interfaces = metaModel.Interfaces.Union(interfaces).ToArray();
            metaModel.Enums = metaModel.Enums.Union(enums).ToArray();
        }

        private static EnumMetadata GetMetaData(EnumDeclarationSyntax syntax,
                                                string sourceFilePath,
                                                IEnumerable<string> usings,
                                                string namespaceName,
                                                ClassMetadata parentClass)
        {
            var res = new EnumMetadata
            {
                Name = syntax.Identifier.Text,
                ParentClass = parentClass,
                SourceFilePath = sourceFilePath,
                Usings = usings,
                Namespace = namespaceName,
                Attributes = GetMetadata(syntax.AttributeLists),
                Modifiers = GetModifiers(syntax.Modifiers)
            };

            res.Members = syntax.Members.Select(m => GetMetadata(m, sourceFilePath, res)).ToImmutableArray();
            return res;
        }

        private static EnumMemberMetadata GetMetadata(EnumMemberDeclarationSyntax syntax,
                                                      string sourceFilePath,
                                                      EnumMetadata parentEnum)
            => new()
            {
                Name = syntax.Identifier.Text,
                SourceFilePath = sourceFilePath,
                Attributes = GetMetadata(syntax.AttributeLists),
                ParentEnum = parentEnum,
                Value = syntax.EqualsValue?.Value is LiteralExpressionSyntax literal ? (int?)literal.Token.Value : null
            };

        private static InterfaceMetadata GetMetadata(InterfaceDeclarationSyntax interfaceSyntax,
                                                     string sourceFilePath,
                                                     IEnumerable<string> usings,
                                                     string namespaceName,
                                                     ClassMetadata parentClass)
            => new()
            {
                Name = interfaceSyntax.Identifier.Text,
                ParentClass = parentClass,
                SourceFilePath = sourceFilePath,
                Usings = usings,
                Namespace = namespaceName,
                Attributes = GetMetadata(interfaceSyntax.AttributeLists),
                Modifiers = GetModifiers(interfaceSyntax.Modifiers)
            };

        private static ClassMetadata GetMetadata(ClassDeclarationSyntax classSyntax,
                                                 string sourceFilePath,
                                                 IEnumerable<string> usings,
                                                 string namespaceName,
                                                 ClassMetadata parentClass,
                                                 SyntaxNode node)
        {

            var implementing = classSyntax.BaseList?.Types.Select(t => GetMetadata(t.Type)).ToImmutableArray() ?? [];

            var classMeta = new ClassMetadata
            {
                Name = classSyntax.Identifier.Text,
                ParentClass = parentClass,
                SourceFilePath = sourceFilePath,
                Usings = usings,
                Namespace = namespaceName,
                ImplementingTypes = implementing,
                Modifiers = GetModifiers(classSyntax.Modifiers),
                Attributes = GetMetadata(classSyntax.AttributeLists)
            };

            var properties = new List<PropertyMetadata>();
            var fields = new List<FieldMetadata>();
            var methods = new List<MethodMetadata>();

            foreach (var member in classSyntax.Members)
            {
                switch (member)
                {
                    case PropertyDeclarationSyntax propertyDeclarationSyntax:
                        properties.Add(GetMetadata(propertyDeclarationSyntax, usings, sourceFilePath, classMeta));
                        break;
                    case FieldDeclarationSyntax fieldDeclarationSyntax:
                        fields.AddRange(GetMetadata(fieldDeclarationSyntax, usings, sourceFilePath, classMeta));
                        break;
                    case MethodDeclarationSyntax methodDeclarationSyntax:
                        methods.Add(GetMetadata(methodDeclarationSyntax, usings, sourceFilePath, classMeta));
                        break;
                }
            }

            classMeta.Properties = [.. properties];
            classMeta.Fields = [.. fields];
            classMeta.Methods = [.. methods];


            foreach (var p in properties)
                p.ParentClass = classMeta;

            FillUpMetaData(node, classMeta, usings, namespaceName, sourceFilePath);
            return classMeta;
        }


        private static PropertyMetadata GetMetadata(PropertyDeclarationSyntax syntax,
                                                    IEnumerable<string> usings,
                                                    string sourceFilePath,
                                                    ClassMetadata ParentClass)
        {
            var nodes = syntax.DescendantNodes();

            SyntaxNode initializer = nodes.OfType<InvocationExpressionSyntax>()
                                                .FirstOrDefault();

            if (initializer is null)
                initializer = nodes.OfType<ArrowExpressionClauseSyntax>()
                                                .FirstOrDefault();


            return new()
            {
                Name = syntax.Identifier.Text,
                ParentClass = ParentClass,
                SourceFilePath = sourceFilePath,
                Type = GetMetadata(syntax.Type),
                Usings = usings,
                Attributes = GetMetadata(syntax.AttributeLists),
                InitializerCode = initializer?.ToFullString(),
                Modifiers = GetModifiers(syntax.Modifiers)
            };
        }

        private static MethodMetadata GetMetadata(MethodDeclarationSyntax syntax,
                                                  IEnumerable<string> usings,
                                                  string sourceFilePath,
                                                  ClassMetadata ParentClass)
            => new()
            {
                Name = syntax.Identifier.Text,
                ParentClass = ParentClass,
                SourceFilePath = sourceFilePath,
                Usings = usings,
                ReturnType = GetMetadata(syntax.ReturnType),
                Attributes = GetMetadata(syntax.AttributeLists),
                Modifiers = GetModifiers(syntax.Modifiers),
                Parameters = GetMetadata(syntax.ParameterList)
            };

        private static IEnumerable<ParameterMetadata> GetMetadata(ParameterListSyntax parameters)
            => parameters.Parameters.Select(p => new ParameterMetadata
            {
                Name = p.Identifier.Text,
                DefaultValue = p.Default?.Value?.ToString(),
                Attributes = GetMetadata(p.AttributeLists),
                Type = GetMetadata(p.Type)
            }).ToImmutableArray();

        private static IEnumerable<FieldMetadata> GetMetadata(FieldDeclarationSyntax syntax,
                                                              IEnumerable<string> usings,
                                                              string sourceFilePath,
                                                              ClassMetadata ParentClass)
        {

            var typeMeta = GetMetadata(syntax.Declaration.Type);
            var attributes = GetMetadata(syntax.AttributeLists);
            return syntax.Declaration
                             .Variables
                             .Select(v => new FieldMetadata()
                             {
                                 Name = v.Identifier.Text,
                                 ParentClass = ParentClass,
                                 SourceFilePath = sourceFilePath,
                                 Usings = usings,
                                 Type = typeMeta,
                                 Attributes = attributes,
                                 Modifiers = GetModifiers(syntax.Modifiers),
                             }).ToImmutableArray();
        }

        private static IEnumerable<AttributeMetadata> GetMetadata(SyntaxList<AttributeListSyntax> attributes)
        {
            var all = attributes.SelectMany(a => a.Attributes)
                                .Select(a => new AttributeMetadata
                                {
                                    Name = a.Name.ToString(),
                                    ArgumentValues = a.ArgumentList?.Arguments.Select(a => a.Expression.ToString()).ToImmutableArray()
                                })
                                .ToImmutableArray();

            return all;
        }

        private static TypeMetadata GetMetadata(TypeSyntax type)
        {
            var res = new TypeMetadata
            {
                FullName = type.ToFullString().Trim()
            };
            switch (type)
            {
                case GenericNameSyntax genericName:
                    res.IsGeneric = true;
                    res.GenericArguments = genericName.TypeArgumentList.Arguments.Select(GetMetadata).ToImmutableArray();
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
            res.Name = res.IdentifierName ?? res.Keyword;

            return res;
        }

        private static IEnumerable<string> GetModifiers(SyntaxTokenList list)
            => list.Select(l => l.Text).ToImmutableArray();

        #endregion
    }
}
