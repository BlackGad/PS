using System;
using System.Collections.Generic;
using System.Linq;
using PS.Query.Data.Predicate.ExpressionBuilder;
using PS.Query.Data.Predicate.Model;

namespace PS.Query.Data.Predicate
{
    internal class PredicateOperators : IPredicateOperators,
                                        IPredicateOperatorsProvider
    {
        private readonly List<Operator> _operators;

        #region Constructors

        public PredicateOperators()
        {
            _operators = new List<Operator>();
        }

        #endregion

        #region IPredicateOperators Members

        public ComplexOperatorBuilder<TResult> Complex<TResult>(string token)
        {
            return new ComplexOperatorBuilder<TResult>(this, token);
        }

        public SimpleOperatorBuilder<TSource> Simple<TSource>(string token)
        {
            return new SimpleOperatorBuilder<TSource>(this, token);
        }

        public IPredicateOperators Register(Operator op)
        {
            _operators.Add(op);
            return this;
        }

        #endregion

        #region IPredicateOperatorsProvider Members

        IEnumerable<ComplexOperator> IPredicateOperatorsProvider.GetComplexOperators()
        {
            return _operators.OfType<ComplexOperator>();
        }

        IEnumerable<SimpleOperator> IPredicateOperatorsProvider.GetOperatorsForType(Type type)
        {
            return _operators.OfType<SimpleOperator>().Where(o => type == o.SourceType);
        }

        #endregion
    }
}