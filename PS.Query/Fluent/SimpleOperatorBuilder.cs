using System;
using System.Linq.Expressions;

namespace PS.Query.Fluent
{
    public abstract class SimpleOperatorBuilder
    {
        #region Constructors

        protected SimpleOperatorBuilder(SchemeOperators schemeOperators, Type sourceType, string token)
        {
            if (schemeOperators == null) throw new ArgumentNullException(nameof(schemeOperators));
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (token == null) throw new ArgumentNullException(nameof(token));

            SchemeOperators = schemeOperators;
            SourceType = sourceType;
            Token = token;
        }

        #endregion

        #region Properties

        protected string OperatorKey { get; set; }

        protected SchemeOperators SchemeOperators { get; }

        protected Type SourceType { get; }

        protected string Token { get; }

        #endregion
    }

    public class SimpleOperatorBuilder<TSource> : SimpleOperatorBuilder
    {
        #region Constructors

        public SimpleOperatorBuilder(SchemeOperators schemeOperators, string token) :
            base(schemeOperators, typeof(TSource), token)
        {
        }

        #endregion

        #region Members

        public SimpleOperatorBuilder<TSource> Key(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            OperatorKey = key;
            return this;
        }

        public SchemeOperators Register(Func<Expression, TSource, BinaryExpression> factory)
        {
            SchemeOperators.Register(new SimpleOperator
            {
                Token = Token,
                SourceType = SourceType,
                ResultType = typeof(bool),
                ExpressionFactory = (member, value) => factory(member, (TSource)value),
                Key = OperatorKey
            });
            return SchemeOperators;
        }

        #endregion
    }
}