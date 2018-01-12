using System;
using System.Linq;
using PS.Navigation;

namespace PS.Expression.Test2
{
    public class ExpressionScheme<TClass>
    {
        #region Constructors

        public ExpressionScheme()
        {
            Operators = new ExpressionSchemeOperators();
            Map = new ExpressionSchemeRoutes<TClass>();
        }

        #endregion

        #region Properties

        public virtual ExpressionSchemeRoutes<TClass> Map { get; }

        public virtual ExpressionSchemeOperators Operators { get; }

        #endregion

        #region Members

        public ExpressionContext CreateReaderContext()
        {
            return new ExpressionContext(Route.Create());
        }

        public bool IsValidOperator(Route routePath, string name)
        {
            var route = Map.GetRoute(routePath);
            if (route == null) return false;

            var availableOperators = Operators.GetOperatorsForType(route.Type);
            availableOperators = availableOperators.Where(o => route.Options.AdditionalOperators.Contains(o.Key) || string.IsNullOrEmpty(o.Key));
            if (!route.Options.IncludeDefaultOperators) availableOperators = availableOperators.Where(o => !string.IsNullOrEmpty(o.Key));

            return availableOperators.Any(o => string.Equals(o.Token, name, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }

    public class ExpressionContext : IDisposable
    {
        private Route _currentRoute;

        #region Constructors

        public ExpressionContext(Route route)
        {
            _currentRoute = route;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}