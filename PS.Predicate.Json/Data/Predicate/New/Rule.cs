using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Data.Parser;

namespace PS.Data.Predicate.New
{
    class Rule<TToken> : IEnumerable where TToken : IToken
    {
        private readonly Lazy<TToken[]> _entryTokens;

        #region Constructors

        public Rule()
        {
            Sequences = new List<RuleSequence<TToken>>();
            _entryTokens = new Lazy<TToken[]>(() => Sequences.SelectMany(s => s.Scanner.EntryTokens).Distinct().ToArray());
        }

        #endregion

        #region Properties

        public TToken[] EntryTokens
        {
            get { return _entryTokens.Value; }
        }

        public List<RuleSequence<TToken>> Sequences { get; }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Sequences.GetEnumerator();
        }

        #endregion

        #region Members

        internal void Add(Expression<Action<Sequence<TToken>>> sequence)
        {
            Sequences.Add(new RuleSequence<TToken>(sequence));
        }

        #endregion
    }
}