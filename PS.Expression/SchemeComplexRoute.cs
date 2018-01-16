using System;
using System.Linq.Expressions;
using PS.Navigation;

namespace PS.Query
{
    public class SchemeComplexRoute : SchemeRoute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SchemeComplexRoute(Route route,
                                  Type type,
                                  SchemeRouteOptions options,
                                  MemberExpression accessor,
                                  SchemeRoutes routes)
            : base(route, type, options, accessor)
        {
            Routes = routes;
        }

        #endregion

        #region Properties

        public SchemeRoutes Routes { get; }

        #endregion
    }
}