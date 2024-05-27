using CodeGeneratorHelpers.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Maui.InternalUtils
{
    internal static class LinqExtendedHelpers
    {

        public static IEnumerable<Match> GetMatches<TModel>(this IEnumerable<TModel> items,
                                                            Regex regex,
                                                            Func<TModel, string> projection)
            => items.Select(i => regex.Match(projection(i)))
                    .Where(r => r.Success)
                    .ToImmutableArray();

        public static IEnumerable<Match> GetMatches(this IEnumerable<string> items,
                                                    Regex regex)
            => items.GetMatches(regex, i => i);

        public static IEnumerable<Match> GetMatches<TModel>(this IEnumerable<TModel> items,
                                                            Regex regex)
            where TModel : ICodeItem
            => items.GetMatches(regex, i => i.Name);
    }
}
