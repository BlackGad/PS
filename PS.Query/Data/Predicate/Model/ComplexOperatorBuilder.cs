using System;
using System.Collections;
using System.Linq.Expressions;

namespace PS.Query.Data.Predicate.Model
{
    public abstract class ComplexOperatorBuilder
    {
        #region Constructors

        protected ComplexOperatorBuilder(IPredicateOperators operators, Type resultType, string token)
        {
            if (operators == null) throw new ArgumentNullException(nameof(operators));
            if (resultType == null) throw new ArgumentNullException(nameof(resultType));
            if (token == null) throw new ArgumentNullException(nameof(token));

            Operators = operators;
            ResultType = resultType;
            Token = token;
        }

        #endregion

        #region Properties

        protected string OperatorKey { get; set; }

        protected IPredicateOperators Operators { get; }

        protected Type ResultType { get; }

        protected string Token { get; }

        #endregion
    }

    public class ComplexOperatorBuilder<TResult> : ComplexOperatorBuilder
    {
        #region Constructors

        public ComplexOperatorBuilder(IPredicateOperators operators, string token) :
            base(operators, typeof(TResult), token)
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

        public IPredicateOperators Register(Func<Expression, LambdaExpression, Expression> factory)
        {
            Operators.Register(new ComplexOperator
            {
                Name = Token,
                SourceType = typeof(IEnumerable),
                ResultType = ResultType,
                ExpressionFactory = factory,
                Key = OperatorKey
            });
            return Operators;
        }

        #endregion
    }
}