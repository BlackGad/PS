using System;
using System.Collections.Generic;
using PS.Query.Data.Predicate.Model;

namespace PS.Query.Data.Predicate.ExpressionBuilder
{
    internal interface IPredicateOperatorsProvider
    {
        #region Members

        IEnumerable<ComplexOperator> GetComplexOperators();
        IEnumerable<SimpleOperator> GetOperatorsForType(Type type);

        #endregion
    }
}