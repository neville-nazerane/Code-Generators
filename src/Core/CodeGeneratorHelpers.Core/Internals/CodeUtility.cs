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

        internal static CodeMetadata GetCodeMetaData(string rawCode, string sourceFilePath = null)
        {
            var metaData = new CodeMetadata
            {
                SourceFilePath = sourceFilePath
            };

            var tree = CSharpSyntaxTree.ParseText(rawCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            FillUpMetaData(root, metaData, null, sourceFilePath);

            return metaData;
        }

        #region private

        private static void FillUpMetaData(SyntaxNode rootNode,
                                           CodeMetadata metaModel,
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


            foreach (var node in rootNode.ChildNodes())
            {
                switch (node)
                {
                    case NamespaceDeclarationSyntax namespaceSyntax:
                        FillUpMetaData(node, metaModel, namespaceSyntax.Name.ToString(), sourceFilePath);
                        break;
                    case ClassDeclarationSyntax classSyntax:
                        classes.Add(GetMetadata(classSyntax, sourceFilePath, namespaceName, parentClass, node));
                        break;

                    case InterfaceDeclarationSyntax interfaceSyntax:
                        interfaces.Add(GetMetadata(interfaceSyntax, sourceFilePath, namespaceName, parentClass));
                        break;

                    case EnumDeclarationSyntax enumSyntax:
                        enums.Add(GetMetaData(enumSyntax, sourceFilePath, namespaceName, parentClass));
                        break;
                }
            }

            metaModel.Classes = metaModel.Classes.Union(classes).ToArray();
            metaModel.Interfaces = metaModel.Interfaces.Union(interfaces).ToArray();
            metaModel.Enums = metaModel.Enums.Union(enums).ToArray();
        }

        private static EnumMetadata GetMetaData(EnumDeclarationSyntax enumSyntax,
                                                string sourceFilePath,
                                                string namespaceName,
                                                ClassMetadata parentClass)
            => new()
            {
                Name = enumSyntax.Identifier.Text,
                ParentClass = parentClass,
                SourceFilePath = sourceFilePath,
                Namespace = namespaceName,
                Attributes = GetMetadata(enumSyntax.AttributeLists),
                Modifiers = GetModifiers(enumSyntax.Modifiers)
            };

        private static InterfaceMetadata GetMetadata(InterfaceDeclarationSyntax interfaceSyntax,
                                                     string sourceFilePath,
                                                     string namespaceName,
                                                     ClassMetadata parentClass)
            => new()
            {
                Name = interfaceSyntax.Identifier.Text,
                ParentClass = parentClass,
                SourceFilePath = sourceFilePath,
                Namespace = namespaceName,
                Attributes = GetMetadata(interfaceSyntax.AttributeLists),
                Modifiers = GetModifiers(interfaceSyntax.Modifiers)
            };
 
        private static ClassMetadata GetMetadata(ClassDeclarationSyntax classSyntax,
                                                 string sourceFilePath,
                                                 string namespaceName,
                                                 ClassMetadata parentClass,
                                                 SyntaxNode node)
        {

            var classMeta = new ClassMetadata
            {
                Name = classSyntax.Identifier.Text,
                ParentClass = parentClass,
                SourceFilePath = sourceFilePath,
                Namespace = namespaceName,
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
                        properties.Add(GetMetadata(propertyDeclarationSyntax, sourceFilePath, classMeta));
                        break;
                    case FieldDeclarationSyntax fieldDeclarationSyntax:
                        fields.AddRange(GetMetadata(fieldDeclarationSyntax, sourceFilePath, classMeta));
                        break;
                    case MethodDeclarationSyntax methodDeclarationSyntax:
                        methods.Add(GetMetadata(methodDeclarationSyntax, sourceFilePath, classMeta));
                        break;
                }
            }

            classMeta.Properties = [.. properties];
            classMeta.Fields = [.. fields];
            classMeta.Methods = [.. methods];


            foreach (var p in properties)
                p.ParentClass = classMeta;

            FillUpMetaData(node, classMeta, namespaceName, sourceFilePath);
            return classMeta;
        }


        private static PropertyMetadata GetMetadata(PropertyDeclarationSyntax syntax,
                                                    string sourceFilePath,
                                                    ClassMetadata ParentClass)
            => new()
            {
                Name = syntax.Identifier.Text,
                ParentClass = ParentClass,
                SourceFilePath = sourceFilePath,
                Attributes = GetMetadata(syntax.AttributeLists),
                Modifiers = GetModifiers(syntax.Modifiers)
            };

        private static MethodMetadata GetMetadata(MethodDeclarationSyntax syntax,
                                                  string sourceFilePath,
                                                  ClassMetadata ParentClass)
            => new()
            {
                Name = syntax.Identifier.Text,
                ParentClass = ParentClass,
                SourceFilePath = sourceFilePath,
                ReturnType = GetMetadata(syntax.ReturnType),
                Attributes = GetMetadata(syntax.AttributeLists),
                Modifiers = GetModifiers(syntax.Modifiers),
                Parameters = GetMetadata(syntax.ParameterList)
            };

        private static IEnumerable<ParameterMetadata> GetMetadata(ParameterListSyntax parameters)
        {
            var res = parameters.Parameters.Select(p => new ParameterMetadata
            {
                Name = p.Identifier.Text,
                Attributes = GetMetadata(p.AttributeLists),
                Type = GetMetadata(p.Type)
            }).ToImmutableArray();

            return res;
        }

        private static IEnumerable<FieldMetadata> GetMetadata(FieldDeclarationSyntax syntax,
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
            var res = new TypeMetadata();
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

            return res;
        }

        private static IEnumerable<string> GetModifiers(SyntaxTokenList list)
            => list.Select(l => l.Text).ToImmutableArray();

        #endregion
    }
}
