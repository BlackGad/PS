using System;
using System.Linq.Expressions;

namespace PS.Query.Data.Predicate.Model
{
    public abstract class SimpleOperatorBuilder
    {
        #region Constructors

        protected SimpleOperatorBuilder(IPredicateOperators operators, Type sourceType, string token)
        {
            if (operators == null) throw new ArgumentNullException(nameof(operators));
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (token == null) throw new ArgumentNullException(nameof(token));

            Operators = operators;
            SourceType = sourceType;
            Token = token;
        }

        #endregion

        #region Properties

        protected string OperatorKey { get; set; }

        protected IPredicateOperators Operators { get; }

        protected Type SourceType { get; }

        protected string Token { get; }

        #endregion
    }

    public class SimpleOperatorBuilder<TSource> : SimpleOperatorBuilder
    {
        #region Constructors

        public SimpleOperatorBuilder(IPredicateOperators operators, string token) :
            base(operators, typeof(TSource), token)
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

        public IPredicateOperators Register(Func<Expression, TSource, BinaryExpression> factory)
        {
            Operators.Register(new SimpleOperator
            {
                Name = Token,
                SourceType = SourceType,
                ResultType = typeof(bool),
                ExpressionFactory = (member, value) => factory(member, (TSource)value),
                Key = OperatorKey
            });
            return Operators;
        }

        #endregion
    }
}