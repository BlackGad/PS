using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Navigation;
using PS.Navigation.Extensions;

namespace PS.Expression.Test2
{
    public class ExpressionSchemeRoutes
    {
        #region Constructors

        public ExpressionSchemeRoutes()
        {
            Routes = new Dictionary<Route, ExpressionSchemeRoute>();
            SubRoutes = new Dictionary<Route, ExpressionSchemeRoutes>();
        }

        #endregion

        #region Properties

        protected Dictionary<Route, ExpressionSchemeRoute> Routes { get; }

        protected Dictionary<Route, ExpressionSchemeRoutes> SubRoutes { get; }

        #endregion

        #region Members

        public ExpressionSchemeRoute GetRoute(Route route)
        {
            return Routes.Where(p => p.Key.AreEqual(route, RouteCaseMode.Insensitive)).Select(p => p.Value).FirstOrDefault();
        }

        public bool IsValidRoute(Route route)
        {
            return Routes.Keys.Any(r => r.StartWith(route, RouteCaseMode.Insensitive));
        }

        #endregion
    }

    public class ExpressionSchemeRoutes<TClass> : ExpressionSchemeRoutes
    {
        #region Members

        public ExpressionSchemeRoutes<TClass> Route<TResult>(Expression<Func<TClass, TResult>> instruction,
                                                             Action<ExpressionSchemeRouteOptions> options = null)
        {
            var expressionBody = instruction?.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            if (Routes.ContainsKey(route)) throw new ArgumentException($"{route} route already declared");

            var optionInstance = new ExpressionSchemeRouteOptions();
            options?.Invoke(optionInstance);
            Routes.Add(route, new ExpressionSchemeRoute(route, typeof(TResult), optionInstance));

            return this;
        }

        public ExpressionSchemeRoutes<TResult> SubRoute<TResult>(Expression<Func<TClass, IEnumerable<TResult>>> instruction,
                                                                 Action<ExpressionSchemeRouteOptions> options = null)
        {
            var expressionBody = instruction?.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            if (SubRoutes.ContainsKey(route)) throw new ArgumentException($"{route} subroute already declared");
            var result = new ExpressionSchemeRoutes<TResult>();
            SubRoutes.Add(route, result);
            return result;
        }

        private Route ExtractRoute(MemberExpression expressionBody)
        {
            if (expressionBody == null) throw new ArgumentException("Invalid expression body");

            var route = Navigation.Route.Create();
            do
            {
                route = Navigation.Route.Create(expressionBody.Member.Name, route);
                if (expressionBody.Expression.NodeType != ExpressionType.MemberAccess) break;
                expressionBody = expressionBody.Expression as MemberExpression;
            } while (expressionBody != null);
            return route;
        }

        #endregion
    }
}