using System;
using System.Collections.Generic;
using System.Linq;
using PS.Data.Predicate.Default;
using PS.Data.Predicate.ExpressionBuilder;
using PS.Data.Predicate.Model;

namespace PS.Data.Predicate
{
    internal class PredicateOperators : IPredicateOperators,
                                        IPredicateOperatorsProvider
    {
        private readonly List<Operator> _operators;

        #region Constructors

        public PredicateOperators()
        {
            _operators = new List<Operator>(Operators.All.SelectMany(s => DescriptorStorage.GetAll(s.GetType())).OfType<Operator>());
        }

        #endregion

        #region IPredicateOperators Members

        public IPredicateOperators Register(Operator op)
        {
            _operators.Add(op);
            return this;
        }

        public IPredicateOperators Reset()
        {
            _operators.Clear();
            return this;
        }

        #endregion

        #region IPredicateOperatorsProvider Members

        IEnumerable<SubsetOperator> IPredicateOperatorsProvider.GetSubsetOperators()
        {
            return _operators.OfType<SubsetOperator>();
        }

        IEnumerable<PredicateOperator> IPredicateOperatorsProvider.GetPredicatesForType(Type type)
        {
            return _operators.OfType<PredicateOperator>().Where(o => type == o.SourceType);
        }

        #endregion
    }
}