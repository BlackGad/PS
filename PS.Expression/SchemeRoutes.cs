﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Navigation;
using PS.Navigation.Extensions;

namespace PS.Query
{
    public abstract class SchemeRoutes
    {
        #region Constructors

        protected SchemeRoutes()
        {
            Routes = new Dictionary<Route, SchemeRoute>();
            ComplexRoutes = new Dictionary<Route, SchemeComplexRoute>();
        }

        #endregion

        #region Properties

        protected Dictionary<Route, SchemeComplexRoute> ComplexRoutes { get; }

        protected Dictionary<Route, SchemeRoute> Routes { get; }

        #endregion

        #region Members

        public SchemeComplexRoute GetComplexRoute(Route route)
        {
            return ComplexRoutes.Where(p => p.Key.AreEqual(route, RouteCaseMode.Insensitive)).
                                 Select(p => p.Value).FirstOrDefault();
            ;
        }

        public SchemeRoute GetRoute(Route route)
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

    public class SchemeRoutes<TClass> : SchemeRoutes
    {
        #region Members

        public SchemeRoutes<TResult> Complex<TResult>(Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                      Action<SchemeRouteOptions> options = null)
        {
            var expressionBody = accessor?.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            return Complex(route, accessor, options);
        }

        public SchemeRoutes<TResult> Complex<TResult>(Route route,
                                                      Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                      Action<SchemeRouteOptions> options = null)
        {
            if (Routes.ContainsKey(route)) throw new ArgumentException($"{route} route already declared");
            var memberAccessExpression = accessor?.Body as MemberExpression;
            if (memberAccessExpression == null) throw new ArgumentException("Member access expression expected as body for accessor");

            var optionInstance = new SchemeRouteOptions();
            options?.Invoke(optionInstance);

            var result = new SchemeRoutes<TResult>();

            var complexRoute = new SchemeComplexRoute(route,
                                                      typeof(TResult),
                                                      optionInstance,
                                                      memberAccessExpression,
                                                      result);
            ComplexRoutes.Add(route, complexRoute);

            return result;
        }

        public SchemeRoutes<TClass> Route<TResult>(Expression<Func<TClass, TResult>> accessor,
                                                   Action<SchemeRouteOptions> options = null)
        {
            var expressionBody = accessor?.Body as MemberExpression;
            var route = ExtractRoute(expressionBody);
            return Route(route, accessor, options);
        }

        public SchemeRoutes<TClass> Route<TResult>(Route route,
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