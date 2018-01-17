using System;
using System.Collections.Generic;
using PS.Query.Configuration;

namespace PS.Query
{
    internal interface ISchemeOperatorProvider
    {
        #region Members

        IEnumerable<ComplexOperator> GetComplexOperators();
        IEnumerable<SimpleOperator> GetOperatorsForType(Type type);

        #endregion
    }
}