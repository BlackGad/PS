using System;
using System.Collections.Generic;
using PS.Data.Predicate.Model;

namespace PS.Data.Predicate.ExpressionBuilder
{
    internal interface IPredicateOperatorsProvider
    {
        #region Members

        IEnumerable<SubsetOperator> GetSubsetOperators();
        IEnumerable<PredicateOperator> GetPredicatesForType(Type type);

        #endregion
    }
}