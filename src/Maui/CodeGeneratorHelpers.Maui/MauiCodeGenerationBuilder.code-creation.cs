using CodeGeneratorHelpers.Core.Models;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Maui;

public partial class MauiCodeGenerationBuilder
{

    internal static string GenerateUtilClass(IEnumerable<ClassMetadata> pages,
                                             IEnumerable<ClassMetadata> viewModels,
                                             string @namespace)
    {

        var usings = pages.Select(p => p.Namespace)
                          .Union(viewModels.Select(p => p.Namespace))
                          .Distinct()
                          .ToImmutableArray();

        var classNames = pages.Select(p => p.Name)
                              .Union(viewModels.Select(p => p.Name))
                              .ToImmutableArray();

        var injections = GenerateTransientInjections(classNames);

        return $@"
{PrintUsings(usings)}

namespace {@namespace};

public static class GenerationUtils
{{
    
    public static IServiceCollection AddGeneratedInjections(this IServiceCollection services)
        => services{string.Join("\n                   ", injections)};

}}

".Trim();
    }

    #region Utility generation

    private static string PrintUsings(IEnumerable<string> usings)
    {
        return string.Join('\n', usings.Select(u => $"using {u};").ToArray());
    }


    internal static IEnumerable<string> GenerateTransientInjections(IEnumerable<string> classNames)
        => classNames.Select(c => $".AddTransient<{c}>()")
                     .ToArray();

    internal static string GenerateInjectionMethod(IEnumerable<string> injections)
        => $@"
    public static IServiceCollection AddGeneratedInjections(this IServiceCollection services)
        => services{string.Join("\n                   ", injections)};
".Trim();


    #endregion
}



