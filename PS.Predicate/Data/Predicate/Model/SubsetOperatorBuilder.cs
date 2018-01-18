using System;
using System.Collections;
using System.Linq.Expressions;

namespace PS.Data.Predicate.Model
{
    public class SubsetOperatorBuilder<TResult>
    {
        private readonly string _name;

        private readonly IPredicateOperators _operators;

        private readonly Type _resultType;

        private string _key;

        #region Constructors

        public SubsetOperatorBuilder(IPredicateOperators operators, string name)
        {
            if (operators == null) throw new ArgumentNullException(nameof(operators));
            if (name == null) throw new ArgumentNullException(nameof(name));
            _resultType = typeof(TResult);
            _operators = operators;
            _name = name;
        }

        #endregion

        #region Members

        public SubsetOperatorBuilder<TResult> Key(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _key = key;
            return this;
        }

        public IPredicateOperators Register(Func<Expression, LambdaExpression, Expression> factory)
        {
            _operators.Register(new SubsetOperator
            {
                Name = _name,
                SourceType = typeof(IEnumerable),
                ResultType = _resultType,
                Expression = factory,
                Key = _key
            });
            return _operators;
        }

        #endregion
    }
}