using System;
using System.Linq.Expressions;
using PS.Data.Parser;

namespace PS.Data.Predicate.New
{
    class RuleSequence<TToken> where TToken : IToken
    {
        readonly Expression<Action<Sequence<TToken>>> _sequenceExpression;

        private BodyScaner<TToken> _scanner;

        #region Constructors

        public RuleSequence(Expression<Action<Sequence<TToken>>> sequenceExpression)
        {
            if (sequenceExpression == null) throw new ArgumentNullException(nameof(sequenceExpression));
            _sequenceExpression = sequenceExpression;
        }

        #endregion

        #region Properties

        public BodyScaner<TToken> Scanner
        {
            get { return _scanner ?? (_scanner = BodyScaner<TToken>.Scan(_sequenceExpression)); }
        }

        #endregion
    }
}