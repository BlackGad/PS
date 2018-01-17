using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Navigation;
using PS.Navigation.Extensions;

namespace PS.Query
{
    internal class SchemeRoutes<TClass> : ISchemeRoutesProvider,
                                          ISchemeRoutesBuilder<TClass>
    {
        #region Constructors

        public SchemeRoutes()
        {
            Routes = new Dictionary<Route, SchemeRoute>();
        }

        #endregion

        #region Properties

        protected Dictionary<Route, SchemeRoute> Routes { get; }

        #endregion

        #region ISchemeRoutesBuilder<TClass> Members

        public ISchemeRoutesBuilder<TResult> Complex<TResult>(Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                              Action<SchemeRouteOptions> options = null)
        {
            var expressionBody = accessor?.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            return Complex(route, accessor, options);
        }

        public ISchemeRoutesBuilder<TResult> Complex<TResult>(Route route,
                                                              Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                              Action<SchemeRouteOptions> options = null)
        {
            if (Routes.ContainsKey(route)) throw new ArgumentException($"{route} route already declared");
            var memberAccessExpression = accessor?.Body as MemberExpression;
            if (memberAccessExpression == null) throw new ArgumentException("Member access expression expected as body for accessor");

            var optionInstance = new SchemeRouteOptions();
            options?.Invoke(optionInstance);

            var result = new SchemeRoutes<TResult>();

            var complexRoute = new SchemeRouteComplex(route,
                                                      typeof(TResult),
                                                      optionInstance,
                                                      memberAccessExpression,
                                                      result);
            Routes.Add(route, complexRoute);

            return result;
        }

        public ISchemeRoutesBuilder<TClass> Route<TResult>(Expression<Func<TClass, TResult>> accessor,
                                                           Action<SchemeRouteOptions> options = null)
        {
            var expressionBody = accessor?.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            return Route(route, accessor, options);
        }

        public ISchemeRoutesBuilder<TClass> Route<TResult>(Route route,
                                                           Expression<Func<TClass, TResult>> accessor,
                                                           Action<SchemeRouteOptions> options = null)
        {
            if (Routes.ContainsKey(route)) throw new ArgumentException($"{route} route already declared");
            var memberAccessExpression = accessor?.Body as MemberExpression;
            if (memberAccessExpression == null) throw new ArgumentException("Member access expression expected as body for accessor");

            var optionInstance = new SchemeRouteOptions();
            options?.Invoke(optionInstance);

            Routes.Add(route, new SchemeRoute(route, typeof(TResult), optionInstance, memberAccessExpression));

            return this;
        }

        #endregion

        #region ISchemeRoutesProvider Members

        public SchemeRouteComplex GetComplexRoute(Route route)
        {
            return Routes.Where(p => p.Key.AreEqual(route, RouteCaseMode.Insensitive) &&
                                     p.Value.GetType() == typeof(SchemeRouteComplex))
                         .Select(p => p.Value)
                         .OfType<SchemeRouteComplex>()
                         .FirstOrDefault();
        }

        public SchemeRoute GetRoute(Route route)
        {
            return Routes.Where(p => p.Key.AreEqual(route, RouteCaseMode.Insensitive) &&
                                     p.Value.GetType() == typeof(SchemeRoute))
                         .Select(p => p.Value)
                         .FirstOrDefault();
        }

        #endregion

        #region Members

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