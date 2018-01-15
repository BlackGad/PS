using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Navigation;
using PS.Navigation.Extensions;

namespace PS.Query
{
    public class ExpressionSchemeRoutes
    {
        #region Constructors

        public ExpressionSchemeRoutes()
        {
            Routes = new Dictionary<Route, ExpressionSchemeRoute>();
            ComplexRoutes = new Dictionary<Route, ExpressionSchemeRoutes>();
        }

        #endregion

        #region Properties

        protected Dictionary<Route, ExpressionSchemeRoutes> ComplexRoutes { get; }

        protected Dictionary<Route, ExpressionSchemeRoute> Routes { get; }

        #endregion

        #region Members

        public ExpressionSchemeRoutes GetComplexRoute(Route route)
        {
            return ComplexRoutes.Where(p => p.Key.AreEqual(route, RouteCaseMode.Insensitive)).Select(p => p.Value).FirstOrDefault();
        }

        public ExpressionSchemeRoute GetRoute(Route route)
        {
            return Routes.Where(p => p.Key.AreEqual(route, RouteCaseMode.Insensitive)).Select(p => p.Value).FirstOrDefault();
        }

        public bool IsValidComplexRoute(Route route)
        {
            return ComplexRoutes.Keys.Any(r => r.StartWith(route, RouteCaseMode.Insensitive));
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

        public ExpressionSchemeRoutes<TResult> Complex<TResult>(Expression<Func<TClass, IEnumerable<TResult>>> instruction,
                                                                Action<ExpressionSchemeRouteOptions> options = null)
        {
            var expressionBody = instruction?.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            if (ComplexRoutes.ContainsKey(route)) throw new ArgumentException($"{route} subroute already declared");
            var result = new ExpressionSchemeRoutes<TResult>();
            ComplexRoutes.Add(route, result);
            return result;
        }

        public ExpressionSchemeRoutes<TClass> Route<TResult>(Expression<Func<TClass, TResult>> accessor,
                                                             Action<ExpressionSchemeRouteOptions> options = null)
        {
            var expressionBody = accessor?.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            return Route(route, accessor, options);
        }

        public ExpressionSchemeRoutes<TClass> Route<TResult>(Route route,
                                                             Expression<Func<TClass, TResult>> accessor,
                                                             Action<ExpressionSchemeRouteOptions> options = null)
        {
            if (Routes.ContainsKey(route)) throw new ArgumentException($"{route} route already declared");
            var memberAccessExpression = accessor.Body as MemberExpression;
            if (memberAccessExpression == null) throw new ArgumentException("Member access expression expected as body for accessor");

            var optionInstance = new ExpressionSchemeRouteOptions();
            options?.Invoke(optionInstance);

            Routes.Add(route, new ExpressionSchemeRoute(route, typeof(TResult), optionInstance, memberAccessExpression));

            return this;
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