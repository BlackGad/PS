using System;
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