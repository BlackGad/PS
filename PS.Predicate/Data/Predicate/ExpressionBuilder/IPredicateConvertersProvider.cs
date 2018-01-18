using System;

namespace PS.Data.Predicate.ExpressionBuilder
{
    internal interface IPredicateConvertersProvider
    {
        #region Members

        object Convert(string value, Type type);

        #endregion
    }
}