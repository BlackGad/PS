using PS.Navigation;

namespace PS.Data.Predicate.ExpressionBuilder
{
    internal interface IPredicateRoutesProvider
    {
        #region Members

        PredicateRouteSubset GetSubsetRoute(Route route);
        PredicateRoute GetRoute(Route route);

        #endregion
    }
}