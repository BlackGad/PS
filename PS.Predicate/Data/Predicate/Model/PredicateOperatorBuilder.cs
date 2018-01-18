using System;
using System.Linq.Expressions;

namespace PS.Data.Predicate.Model
{
    public class PredicateOperatorBuilder<TSource>
    {
        private readonly string _name;
        private readonly IPredicateOperators _operators;

        private readonly Type _sourceType;

        private string _key;

        #region Constructors

        public PredicateOperatorBuilder(IPredicateOperators operators, string name)
        {
            _operators = operators;
            _sourceType = typeof(TSource);
            _name = name;
            _operators = operators;
        }

        #endregion

        #region Members

        public PredicateOperatorBuilder<TSource> Key(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _key = key;
            return this;
        }

        public IPredicateOperators Register(Func<Expression, Type, TSource, BinaryExpression> factory)
        {
            _operators.Register(new PredicateOperator
            {
                Name = _name,
                SourceType = _sourceType,
                ResultType = typeof(bool),
                Expression = (member, type, value) => factory(member, typeof(TSource), (TSource)value),
                Key = _key
            });
            return _operators;
        }

        #endregion
    }
}