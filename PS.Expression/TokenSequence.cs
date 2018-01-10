using System;
using System.Collections.Generic;

namespace PS.Expression
{
    public class TokenSequence<TToken>
    {
        private readonly List<TToken> _tokens;
        private int _currentPosition;

        #region Constructors

        public TokenSequence(IEnumerable<TToken> tokens)
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));
            _tokens = new List<TToken>(tokens);
            _currentPosition = 0;
        }

        #endregion

        #region Members

        public bool IsTokenAvailable(int level = 0)
        {
            return _currentPosition + level < _tokens.Count;
        }

        public TToken Lookahead(int level = 0)
        {
            if (!IsTokenAvailable(level)) return default(TToken);
            return _tokens[_currentPosition + level];
        }

        public TokenSequence<TToken> Next()
        {
            _currentPosition++;
            return this;
        }

        #endregion
    }
}