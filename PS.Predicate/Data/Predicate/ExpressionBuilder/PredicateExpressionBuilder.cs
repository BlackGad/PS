using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PS.Data.Logic;
using PS.Data.Logic.Extensions;
using PS.Data.Predicate.Logic;
using PS.Data.Predicate.Model;
using PS.Linq.Expressions;

namespace PS.Data.Predicate.ExpressionBuilder
{
    internal static class PredicateExpressionBuilder
    {
        #region Static members

        internal static Expression<Func<TClass, bool>> Build<TClass>(IPredicateSchemeProvider scheme, IExpression expression)
        {
            var lambda = Build(scheme, typeof(TClass), scheme.Routes, expression);
            return (Expression<Func<TClass, bool>>)lambda;
        }

        private static LambdaExpression Build(IPredicateSchemeProvider scheme,
                                              Type paramType,
                                              IPredicateRoutesProvider schemeRoutes,
                                              IExpression source)
        {
            if (paramType == null) throw new ArgumentNullException(nameof(paramType));

            var factorized = FactorizeExpression(source);
            var p = Expression.Parameter(paramType);
            Expression body = null;
            foreach (var logicalExpression in factorized.Expressions.OfType<ILogicalExpression>()) //Or
            {
                Expression intermediateResult = null;
                foreach (var expression in logicalExpression.Expressions.OfType<RouteExpression>()) //And
                {
                    var routeSubsetExpression = expression as RouteSubsetExpression;
                    if (routeSubsetExpression != null)
                    {
                        var route = schemeRoutes.GetSubsetRoute(expression.Route);
                        if (route == null) throw new ArgumentException($"Cannot process '{routeSubsetExpression.Route}' subset route");

                        var subsetOperator = GeSubsetOperator(scheme, route, routeSubsetExpression.Query);
                        if (subsetOperator == null)
                        {
                            var message = $"'{expression.Route}' subset route does not support '{routeSubsetExpression.Query}' operator";
                            throw new ArgumentException(message);
                        }

                        var subExpression = Build(scheme, route.Type, route.Routes, routeSubsetExpression.Subset);

                        var accessor = ParameterReplacer.Replace(p, route.Accessor);
                        var compiledExpression = subsetOperator.Expression(accessor, subExpression);

                        var operatorExpression = expression.Operator;
                        var sourceType = compiledExpression.Type;

                        if (operatorExpression != null)
                        {
                            var @operator = GetOperator(scheme, sourceType, route.Options, operatorExpression.Name);
                            if (@operator == null)
                            {
                                var message = $"'{expression.Route}' route does not support '{operatorExpression.Name}' operator";
                                throw new ArgumentException(message);
                            }

                            var stringValue = operatorExpression.Value.Trim('\"').Trim('\'');
                            var value = scheme.Converters.Convert(stringValue, sourceType);
                            compiledExpression = @operator.Expression(compiledExpression, sourceType, value);
                            if (operatorExpression.Inverted) compiledExpression = Expression.Not(compiledExpression);
                        }

                        intermediateResult = intermediateResult == null
                            ? compiledExpression
                            : Expression.AndAlso(intermediateResult, compiledExpression);
                    }
                    else
                    {
                        var route = schemeRoutes.GetRoute(expression.Route);
                        if (route == null)
                        {
                            var message = $"Unknown '{expression.Route}' route in current context";
                            throw new ArgumentException(message);
                        }

                        var @operator = GetOperator(scheme, route.Type, route.Options, expression.Operator.Name);
                        if (@operator == null)
                        {
                            var message = $"'{expression.Route}' route does not support '{expression.Operator.Name}' operator";
                            throw new ArgumentException(message);
                        }

                        var accessor = ParameterReplacer.Replace(p, route.Accessor);
                        var stringValue = expression.Operator.Value.Trim('\"').Trim('\'');
                        var value = scheme.Converters.Convert(stringValue, route.Type);
                        Expression compiledExpression = @operator.Expression(accessor, route.Type, value);

                        if (expression.Operator.Inverted) compiledExpression = Expression.Not(compiledExpression);

                        intermediateResult = intermediateResult == null
                            ? compiledExpression
                            : Expression.AndAlso(intermediateResult, compiledExpression);
                    }
                }

                if (intermediateResult != null)
                {
                    body = body == null
                        ? intermediateResult
                        : Expression.OrElse(body, intermediateResult);
                }
            }
            body = body ?? Expression.Constant(true);
            var lambda = Expression.Lambda(body, p);
            return lambda;
        }

        private static ILogicalExpression FactorizeExpression(IExpression expression)
        {
            return expression.Factorize(new FactorizeParams((o, inverted) => o));
        }

        private static SubsetOperator GeSubsetOperator(IPredicateSchemeProvider scheme, PredicateRouteSubset route, string name)
        {
            if (route == null) return null;
            return SelectOperator(scheme.Operators.GetSubsetOperators().ToList(), route.Options, name);
        }

        private static PredicateOperator GetOperator(IPredicateSchemeProvider scheme, Type sourceType, PredicateRouteOptions options, string name)
        {
            return SelectOperator(scheme.Operators.GetPredicatesForType(sourceType).ToList(), options, name);
        }

        private static TOperator SelectOperator<TOperator>(IList<TOperator> allOperators, PredicateRouteOptions options, string name)
            where TOperator : Operator
        {
            var availableOperators = allOperators.Except(allOperators.Where(o => !string.IsNullOrEmpty(o.Key) &&
                                                                                 !options.AdditionalOperators.Contains(o.Key)));
            if (!options.IncludeDefaultOperators)
                availableOperators = availableOperators.Except(allOperators.Where(o => string.IsNullOrEmpty(o.Key)));
            else
                availableOperators = availableOperators.Except(allOperators.Where(o => string.IsNullOrEmpty(o.Key) &&
                                                                                       options.ExcludeDefaultOperators.Contains(o.Name)));

            return availableOperators.FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}