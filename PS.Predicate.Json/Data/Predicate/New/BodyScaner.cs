using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Data.Parser;

namespace PS.Data.Predicate.New
{
    class BodyScaner<TToken> : ExpressionVisitor where TToken : IToken
    {
        #region Static members

        public static BodyScaner<TToken> Scan(Expression<Action<Sequence<TToken>>> expression)
        {
            var scaner = new BodyScaner<TToken>();
            scaner.Visit(expression);
            return scaner;
        }

        #endregion

        private Lazy<TToken[]> _entryTokenFunc;

        #region Constructors

        public BodyScaner()
        {
            Tokens = new List<TToken>();
            Rules = new List<Rule<TToken>>();
        }

        #endregion

        #region Properties

        public TToken[] EntryTokens
        {
            get
            {
                if (_entryTokenFunc == null) return Enumerable.Empty<TToken>().ToArray();
                return _entryTokenFunc.Value;
            }
        }

        public List<Rule<TToken>> Rules { get; }

        public List<TToken> Tokens { get; }

        #endregion

        #region Override members

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == typeof(Sequence<TToken>).GetMethod(nameof(Sequence<TToken>.Token)))
            {
                var arg = node.Arguments.First();
                var token = (TToken)Expression.Lambda(arg).Compile().DynamicInvoke();
                _entryTokenFunc = new Lazy<TToken[]>(() => new[] { token });
                Tokens.Add(token);
            }

            if (node.Method == typeof(Sequence<TToken>).GetMethod(nameof(Sequence<TToken>.Rule)))
            {
                var arg = node.Arguments.First();
                var rule = (Rule<TToken>)Expression.Lambda(arg).Compile().DynamicInvoke();
                _entryTokenFunc = new Lazy<TToken[]>(() => rule.Sequences
                                                               .SelectMany(s => s.Scanner.EntryTokens)
                                                               .ToArray());
                Rules.Add(rule);
            }

            return base.VisitMethodCall(node);
        }

        #endregion
    }
}