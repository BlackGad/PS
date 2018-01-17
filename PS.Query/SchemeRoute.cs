using System;
using System.Linq.Expressions;
using PS.Navigation;

namespace PS.Query
{
    internal class SchemeRoute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SchemeRoute(Route route, Type type, SchemeRouteOptions options, MemberExpression accessor)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            Options = options;
            Accessor = accessor;
            Route = route;
            Type = type;
        }

        #endregion

        #region Properties

        public MemberExpression Accessor { get; }
        public SchemeRouteOptions Options { get; }
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