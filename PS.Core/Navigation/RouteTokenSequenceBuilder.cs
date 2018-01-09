using System.Collections.Generic;
using PS.Extensions;

namespace PS.Navigation
{
    internal class RouteTokenSequenceBuilder
    {
        #region Constants

        private static readonly RouteToken Wildcard;
        private static readonly RouteToken WildcardRecursive;

        #endregion

        private readonly List<RouteToken> _records;
        private readonly List<string> _regexInputTokens;
        private readonly List<string> _regexPatternTokens;
        private int _hash;
        private int? _recursiveStart;
        private int? _recursiveEnd;

        #region Constructors

        static RouteTokenSequenceBuilder()
        {
            Wildcard = Route.EnsureTokenCached(Route.Wildcard);
            WildcardRecursive = Route.EnsureTokenCached(Route.WildcardRecursive);
        }

        public RouteTokenSequenceBuilder()
        {
            _regexPatternTokens = new List<string>();
            _regexInputTokens = new List<string>();
            _records = new List<RouteToken>();
        }

        #endregion

        #region Members

        public void Add(RouteToken record)
        {
            _records.Add(record);

            var recordIndex = _records.Count;
            _hash = _hash.MergeHash(record.Hash);

            var recordTokenString = record.Token.ToString();
            if (Equals(record, Wildcard))
            {
                _regexPatternTokens.Add("(?<" + recordIndex + ">\\d+?)");
                if(!_recursiveStart.HasValue) _recursiveStart = recordIndex - 1;
                _recursiveEnd = recordIndex;
            }
            else if (Equals(record, WildcardRecursive))
            {
                _regexPatternTokens.Add("(?<" + recordIndex + ">.+?)");
                if (!_recursiveStart.HasValue) _recursiveStart = recordIndex - 1;
                _recursiveEnd = recordIndex;
            }
            else _regexPatternTokens.Add("(?<" + recordIndex + ">" + recordTokenString + ")");

            _regexInputTokens.Add(recordTokenString);
        }

        public RouteTokenSequence Build()
        {
            return new RouteTokenSequence(_records,
                                          _hash,
                                          _recursiveStart,
                                          _recursiveEnd,
                                          string.Join("/", _regexInputTokens),
                                          string.Join("/", _regexPatternTokens));
        }

        #endregion
    }
}