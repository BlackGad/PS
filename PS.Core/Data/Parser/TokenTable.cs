using System;
using System.Collections;
using System.Collections.Generic;

namespace PS.Data.Parser
{
    public class TokenTable<TToken> : IEnumerable<TToken> where TToken : IToken
    {
        private readonly Dictionary<string, TToken> _table;

        #region Constructors

        public TokenTable()
        {
            _table = new Dictionary<string, TToken>();
        }

        #endregion

        #region Properties

        public TToken this[string key]
        {
            get
            {
                if (_table.ContainsKey(key)) return _table[key];
                return default(TToken);
            }
        }

        #endregion

        #region IEnumerable<TToken> Members

        public IEnumerator<TToken> GetEnumerator()
        {
            return _table.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Members

        public TokenTable<TToken> Add(string key, TToken token)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Invalid key");
            if (token == null) throw new ArgumentNullException(nameof(token));
            _table.Add(key, token);
            return this;
        }

        #endregion
    }
}