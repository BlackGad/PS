using System;
using System.Linq;
using System.Linq.Expressions;
using PS.Query.Logic;
using PS.Query.Logic.Extensions;
using PS.Query.Model;
using LogicalExpression = PS.Query.Model.LogicalExpression;

namespace PS.Query
{
    public class ExpressionScheme<TClass>
    {
        #region Static members

        private static ILogicalExpression FactorizeExpression(LogicalExpression expression)
        {
            return expression.Factorize(new FactorizeParams((o, inverted) => o));
        }

        #endregion

        #region Constructors

        public ExpressionScheme()
        {
            Operators = new SchemeOperators();
            Map = new SchemeRoutes<TClass>();
            Converters = new SchemeConverters();
        }

        #endregion

        #region Properties

        public SchemeConverters Converters { get; }

        public SchemeRoutes<TClass> Map { get; }

        public SchemeOperators Operators { get; }

        #endregion

        #region Members

        public Func<TClass, bool> Build(IExpressionProvider provider)
        {
            var lambda = Build(typeof(TClass), Map, provider.Provide());
            return (Func<TClass, bool>)lambda.Compile();
        }

        internal ComplexOperator GetComplexOperator(SchemeComplexRoute route, string name)
        {
            if (route == null) return null;

            var availableOperators = Operators.GetComplexOperators();
            availableOperators = availableOperators.Where(o => route.Options.AdditionalOperators.Contains(o.Key) || string.IsNullOrEmpty(o.Key));
            if (!route.Options.IncludeDefaultOperators) availableOperators = availableOperators.Where(o => !string.IsNullOrEmpty(o.Key));

            return availableOperators.FirstOrDefault(o => string.Equals(o.Token, name, StringComparison.InvariantCultureIgnoreCase));
        }

        internal SimpleOperator GetOperator(Type sourceType, SchemeRouteOptions options, string name)
        {
            var availableOperators = Operators.GetOperatorsForType(sourceType);
            availableOperators = availableOperators.Where(o => options.AdditionalOperators.Contains(o.Key) || string.IsNullOrEmpty(o.Key));
            if (!options.IncludeDefaultOperators) availableOperators = availableOperators.Where(o => !string.IsNullOrEmpty(o.Key));

            return availableOperators.FirstOrDefault(o => string.Equals(o.Token, name, StringComparison.InvariantCultureIgnoreCase));
        }

        private LambdaExpression Build(Type paramType, SchemeRoutes schemeRoutes, LogicalExpression source)
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
                    var complexExpression = expression as ComplexRouteExpression;
                    if (complexExpression != null)
                    {
                        var route = schemeRoutes.GetComplexRoute(expression.Route);
                        if (route == null) throw new ArgumentException($"Cannot process '{complexExpression.Route}' complex route");

                        var complexOperator = GetComplexOperator(route, complexExpression.ComplexOperator);
                        if (complexOperator == null)
                        {
                            var message = $"'{expression.Route}' complex route does not support '{complexExpression.ComplexOperator}' operator";
                            throw new ArgumentException(message);
                        }

                        var subExpression = Build(route.Type, route.Routes, complexExpression.Sub);

                        var accessor = ParameterReplacer.Replace(p, route.Accessor);
                        var compiledExpression = complexOperator.ExpressionFactory(accessor, subExpression);

                        var operatorExpression = expression.Operator;
                        var sourceType = compiledExpression.Type;

                        if (operatorExpression != null)
                        {
                            var @operator = GetOperator(sourceType, route.Options, operatorExpression.Operator);
                            if (@operator == null)
                            {
                                var message = $"'{expression.Route}' route does not support '{operatorExpression.Operator}' operator";
                                throw new ArgumentException(message);
                            }

                            var stringValue = operatorExpression.Value.Trim('\"').Trim('\'');
                            var value = Converters.Convert(stringValue, sourceType);
                            compiledExpression = @operator.ExpressionFactory(compiledExpression, value);
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

                        var @operator = GetOperator(route.Type, route.Options, expression.Operator.Operator);
                        if (@operator == null)
                        {
                            var message = $"'{expression.Route}' route does not support '{expression.Operator.Operator}' operator";
                            throw new ArgumentException(message);
                        }

                        var accessor = ParameterReplacer.Replace(p, route.Accessor);
                        var stringValue = expression.Operator.Value.Trim('\"').Trim('\'');
                        var value = Converters.Convert(stringValue, route.Type);
                        Expression compiledExpression = @operator.ExpressionFactory(accessor, value);

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

        #endregion
    }
}