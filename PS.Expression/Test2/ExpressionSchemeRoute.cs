using System;
using PS.Navigation;

namespace PS.Expression.Test2
{
    public class ExpressionSchemeRoute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ExpressionSchemeRoute(Route route, Type type, ExpressionSchemeRouteOptions options)
        {
            Options = options;
            Route = route;
            Type = type;
        }

        #endregion

        #region Properties

        public ExpressionSchemeRouteOptions Options { get; }
        public Route Route { get; }
        public Type Type { get; }

        #endregion

        #region Override members

        public override string ToString()
        {
            return $"{Type.Name}: {Route}";
        }

        #endregion
    }
}