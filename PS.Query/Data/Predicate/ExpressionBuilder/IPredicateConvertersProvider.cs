using System;

namespace PS.Query.Data.Predicate.ExpressionBuilder
{
    internal interface IPredicateConvertersProvider
    {
        #region Members

        object Convert(string value, Type type);

        #endregion
    }
}