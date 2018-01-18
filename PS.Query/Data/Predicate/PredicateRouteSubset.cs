using System;
using System.Linq.Expressions;
using PS.Navigation;
using PS.Query.Data.Predicate.ExpressionBuilder;

namespace PS.Query.Data.Predicate
{
    internal class PredicateRouteSubset : PredicateRoute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public PredicateRouteSubset(Route route,
                                    Type type,
                                    PredicateRouteOptions options,
                                    MemberExpression accessor,
                                    IPredicateRoutesProvider routes)
            : base(route, type, options, accessor)
        {
            Routes = routes;
        }

        #endregion

        #region Properties

        public IPredicateRoutesProvider Routes { get; }

        #endregion
    }
}