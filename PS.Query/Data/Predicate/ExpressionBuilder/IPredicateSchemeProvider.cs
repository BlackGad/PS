namespace PS.Query.Data.Predicate.ExpressionBuilder
{
    internal interface IPredicateSchemeProvider
    {
        #region Properties

        IPredicateConvertersProvider Converters { get; }

        IPredicateOperatorsProvider Operators { get; }

        IPredicateRoutesProvider Routes { get; }

        #endregion
    }
}