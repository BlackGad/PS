using System;
using System.Linq.Expressions;
using PS.Navigation;

namespace PS.Query
{
    internal class SchemeRouteComplex : SchemeRoute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SchemeRouteComplex(Route route,
                                  Type type,
                                  SchemeRouteOptions options,
                                  MemberExpression accessor,
                                  ISchemeRoutesProvider routes)
            : base(route, type, options, accessor)
        {
            Routes = routes;
        }

        #endregion

        #region Properties

        public ISchemeRoutesProvider Routes { get; }

        #endregion
    }
}