using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PS.Navigation;

namespace PS.Data.Predicate.Extensions
{
    public static class PredicateRoutesExtensions
    {
        #region Static members

        public static IPredicateRoutes<TClass> Route<TClass, TResult>(this IPredicateRoutes<TClass> routes,
                                                                      Expression<Func<TClass, TResult>> accessor,
                                                                      Action<PredicateRouteOptions> options = null)
        {
            if (routes == null) throw new ArgumentNullException(nameof(routes));
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            var expressionBody = accessor.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            return routes.Route(route, accessor, options);
        }

        public static IPredicateRoutes<TResult> Subset<TClass, TResult>(this IPredicateRoutes<TClass> routes,
                                                                        Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                                        Action<PredicateRouteOptions> options = null)
        {
            if (routes == null) throw new ArgumentNullException(nameof(routes));
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));

            var expressionBody = accessor.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            return routes.Subset(route, accessor, options);
        }

        private static Route ExtractRoute(MemberExpression expressionBody)
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