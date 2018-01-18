using PS.Navigation;

namespace PS.Query.Data.Predicate.ExpressionBuilder
{
    internal interface IPredicateRoutesProvider
    {
        #region Members

        PredicateRouteComplex GetComplexRoute(Route route);
        PredicateRoute GetRoute(Route route);

        #endregion
    }
}