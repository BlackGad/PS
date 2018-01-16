using System;
using System.Collections;
using System.Linq.Expressions;

namespace PS.Query.Fluent
{
    public abstract class ComplexOperatorBuilder
    {
        #region Constructors

        protected ComplexOperatorBuilder(SchemeOperators schemeOperators, Type resultType, string token)
        {
            if (schemeOperators == null) throw new ArgumentNullException(nameof(schemeOperators));
            if (resultType == null) throw new ArgumentNullException(nameof(resultType));
            if (token == null) throw new ArgumentNullException(nameof(token));

            SchemeOperators = schemeOperators;
            ResultType = resultType;
            Token = token;
        }

        #endregion

        #region Properties

        protected string OperatorKey { get; set; }

        protected Type ResultType { get; }

        protected SchemeOperators SchemeOperators { get; }

        protected string Token { get; }

        #endregion
    }

    public class ComplexOperatorBuilder<TResult> : ComplexOperatorBuilder
    {
        #region Constructors

        public ComplexOperatorBuilder(SchemeOperators schemeOperators, string token) :
            base(schemeOperators, typeof(TResult), token)
        {
        }

        #endregion

        #region Members

        public ComplexOperatorBuilder<TResult> Key(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            OperatorKey = key;
            return this;
        }

        public SchemeOperators Register(Func<Expression, LambdaExpression, Expression> factory)
        {
            SchemeOperators.Register(new ComplexOperator
            {
                Token = Token,
                SourceType = typeof(IEnumerable),
                ResultType = ResultType,
                ExpressionFactory = factory,
                Key = OperatorKey
            });
            return SchemeOperators;
        }

        #endregion
    }
}