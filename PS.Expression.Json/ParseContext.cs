using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PS.Expression.Json
{
    public class ParseContext
    {
        private readonly int _offset;

        private readonly List<ParseSequenceCheck> _sequenceChecks;
        private readonly List<JsonToken> _tokens;

        #region Constructors

        public ParseContext(IEnumerable<JsonToken> tokens) : this()
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));
            _tokens = new List<JsonToken>(tokens);
        }

        private ParseContext()
        {
            _sequenceChecks = new List<ParseSequenceCheck>();
            Environment = new Dictionary<object, object>();
        }

        private ParseContext(List<JsonToken> tokens, int offset) : this()
        {
            _tokens = tokens;
            _offset = offset;
        }

        #endregion

        #region Properties

        public Dictionary<object, object> Environment { get; }

        public ParseSequenceCheck FailedBranch
        {
            get
            {
                if (!_sequenceChecks.Any() || SuccessBranch != null) return null;
                return _sequenceChecks.OrderByDescending(check => check.GetAssertsLength()).First();
            }
        }

        public ParseSequenceCheck SuccessBranch
        {
            get { return _sequenceChecks.FirstOrDefault(c => c.Error == null); }
        }

        #endregion

        #region Members

        public ParseContext Branch(int offset)
        {
            var branch = new ParseContext(_tokens, _offset + offset);
            foreach (var pair in Environment)
            {
                branch.Environment.Add(pair.Key, pair.Value);
            }

            return branch;
        }

        public ParseSequenceCheck CheckSequence(string ruleName, [CallerMemberName] string sourceRuleName = null)
        {
            if (_sequenceChecks.Any(s => s.Error == null)) return new BlankParseSequenceCheck(this, sourceRuleName);

            var sequenceCheck = new ParseSequenceCheck(this, sourceRuleName);
            _sequenceChecks.Add(sequenceCheck);
            return sequenceCheck;
        }

        public JsonToken GetToken(int position = 0)
        {
            if (_offset + position >= 0 && _offset + position < _tokens.Count) return _tokens[_offset + position];
            return default(JsonToken);
        }

        #endregion
    }
}