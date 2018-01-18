using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Navigation;
using PS.Navigation.Extensions;
using PS.Query.Data.Predicate.ExpressionBuilder;

namespace PS.Query.Data.Predicate
{
    internal class PredicateRoutes<TClass> : IPredicateRoutesProvider,
                                             IPredicateRoutes<TClass>
    {
        #region Constructors

        public PredicateRoutes()
        {
            Routes = new Dictionary<Route, PredicateRoute>();
        }

        #endregion

        #region Properties

        protected Dictionary<Route, PredicateRoute> Routes { get; }

        #endregion

        #region IPredicateRoutes<TClass> Members

        public IPredicateRoutes<TResult> Complex<TResult>(Route route,
                                                          Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                          Action<PredicateRouteOptions> options = null)
        {
            if (Routes.ContainsKey(route)) throw new ArgumentException($"{route} route already declared");
            var memberAccessExpression = accessor?.Body as MemberExpression;
            if (memberAccessExpression == null) throw new ArgumentException("Member access expression expected as body for accessor");

            var optionInstance = new PredicateRouteOptions();
            options?.Invoke(optionInstance);

            var result = new PredicateRoutes<TResult>();

            var complexRoute = new PredicateRouteComplex(route,
                                                      typeof(TResult),
                                                      optionInstance,
                                                      memberAccessExpression,
                                                      result);
            Routes.Add(route, complexRoute);

            return result;
        }

        public IPredicateRoutes<TClass> Route<TResult>(Route route,
                                                       Expression<Func<TClass, TResult>> accessor,
                                                       Action<PredicateRouteOptions> options = null)
        {
            if (Routes.ContainsKey(route)) throw new ArgumentException($"{route} route already declared");
            var memberAccessExpression = accessor?.Body as MemberExpression;
            if (memberAccessExpression == null) throw new ArgumentException("Member access expression expected as body for accessor");

            var optionInstance = new PredicateRouteOptions();
            options?.Invoke(optionInstance);

            Routes.Add(route, new PredicateRoute(route, typeof(TResult), optionInstance, memberAccessExpression));

            return this;
        }

        #endregion

        #region IPredicateRoutesProvider Members

        public PredicateRouteComplex GetComplexRoute(Route route)
        {
            return Routes.Where(p => p.Key.AreEqual(route, RouteCaseMode.Insensitive) &&
                                     p.Value.GetType() == typeof(PredicateRouteComplex))
                         .Select(p => p.Value)
                         .OfType<PredicateRouteComplex>()
                         .FirstOrDefault();
        }

        public PredicateRoute GetRoute(Route route)
        {
            return Routes.Where(p => p.Key.AreEqual(route, RouteCaseMode.Insensitive) &&
                                     p.Value.GetType() == typeof(PredicateRoute))
                         .Select(p => p.Value)
                         .FirstOrDefault();
        }

        #endregion
    }
}