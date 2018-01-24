using System.Linq;
using System.Text.RegularExpressions;
using PS.Extensions;

namespace PS.Navigation.Extensions
{
    public static class RouteExtensions
    {
        #region Static members

        public static bool Contains(this Route source, Route route, RouteCaseMode caseSensitive = RouteCaseMode.Sensitive)
        {
            if (source.IsEmpty() || route.IsEmpty()) return false;

            var modeIndex = (int)caseSensitive;
            if (modeIndex > 1) return false;

            return Regex.IsMatch(source.Sequences[modeIndex].RegexInput, route.Sequences[modeIndex].RegexPattern);
        }

        public static bool EndWith(this Route source, Route route, RouteCaseMode caseSensitive = RouteCaseMode.Sensitive)
        {
            if (source.IsEmpty() || route.IsEmpty()) return false;

            var modeIndex = (int)caseSensitive;
            if (modeIndex > 1) return false;

            return Regex.IsMatch(source.Sequences[modeIndex].RegexInput,
                                 route.Sequences[modeIndex].RegexPattern + "$");
        }

        public static bool AreEqual(this Route source, Route route, RouteCaseMode caseSensitive = RouteCaseMode.Sensitive)
        {
            if (source.IsEmpty() || route.IsEmpty()) return false;

            var modeIndex = (int)caseSensitive;
            if (modeIndex > 1) return false;

            return Regex.IsMatch(source.Sequences[modeIndex].RegexInput,
                                 "^" + route.Sequences[modeIndex].RegexPattern + "$");
        }

        public static bool IsEmpty(this Route source)
        {
            return (source?.Count ?? 0) == 0;
        }

        public static RouteRecursiveSplit RecursiveSplit(this Route source, Route mask, RouteCaseMode caseSensitive = RouteCaseMode.Sensitive)
        {
            if (source.IsEmpty() || mask.IsEmpty()) return null;

            var modeIndex = (int)caseSensitive;
            if (modeIndex > 1) return null;
            var maskTokenSequence = mask.Sequences[modeIndex];

            var sourceRegexInput = source.Sequences[modeIndex].RegexInput;
            var match = Regex.Match(sourceRegexInput, maskTokenSequence.RegexPattern);
            if (!match.Success) return null;

            // ReSharper disable PossibleInvalidOperationException
            var recursiveStart = maskTokenSequence.RecursiveStart ?? maskTokenSequence.Count;
            var postfixStart = maskTokenSequence.RecursiveEnd ?? maskTokenSequence.Count;
            // ReSharper restore PossibleInvalidOperationException

            var wholeMatch = match.Groups.Enumerate<Group>().First();

            var capturedGroups = match.Groups.Enumerate<Group>().Skip(1).ToList();

            var prefixTokens = sourceRegexInput.Substring(0, wholeMatch.Index) + string.Join("/", capturedGroups.Take(recursiveStart).Select(g => g.Value));
            var recursiveTokens = string.Join("/", capturedGroups.Skip(recursiveStart).Take(postfixStart - recursiveStart).Select(g => g.Value));

            var sourceRecursiveStart = prefixTokens.Occurrences('/') + 1;
            var sourcePostfixStart = sourceRecursiveStart + (string.IsNullOrEmpty(recursiveTokens) ? 0 : recursiveTokens.Occurrences('/') + 1);

            var prefix = source.Sub(0, sourceRecursiveStart);
            var recursive = source.Sub(sourceRecursiveStart, sourcePostfixStart - sourceRecursiveStart);
            var postfix = source.Sub(sourcePostfixStart, source.Count - sourcePostfixStart);

            return new RouteRecursiveSplit(prefix, recursive, postfix);
        }

        public static bool StartWith(this Route source, Route route, RouteCaseMode caseSensitive = RouteCaseMode.Sensitive)
        {
            if (source.IsEmpty() || route.IsEmpty()) return false;

            var modeIndex = (int)caseSensitive;
            if (modeIndex > 1) return false;

            return Regex.IsMatch(source.Sequences[modeIndex].RegexInput, "^" + route.Sequences[modeIndex].RegexPattern);
        }

        public static Route Sub(this Route source, int skip, int take)
        {
            if (source.IsEmpty()) return Routes.Empty;
            return Route.Create(source.Skip(skip).Take(take));
        }

        #endregion
    }
}