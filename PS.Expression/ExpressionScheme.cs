using System;
using System.Linq;
using System.Linq.Expressions;
using PS.Navigation;
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
            Operators = new ExpressionSchemeOperators();
            Map = new ExpressionSchemeRoutes<TClass>();
            Converters = new ExpressionSchemeConverters();
        }

        #endregion

        #region Properties

        public ExpressionSchemeConverters Converters { get; }

        public ExpressionSchemeRoutes<TClass> Map { get; }

        public ExpressionSchemeOperators Operators { get; }

        #endregion

        #region Members

        public Func<TClass, bool> Build(IExpressionProvider provider)
        {
            return Build(provider.Provide());
        }

        public ExpressionOperator GetOperator(Route routePath, string name)
        {
            var route = Map.GetRoute(routePath);
            if (route == null) return null;

            var availableOperators = Operators.GetOperatorsForType(route.Type);
            availableOperators = availableOperators.Where(o => route.Options.AdditionalOperators.Contains(o.Key) || string.IsNullOrEmpty(o.Key));
            if (!route.Options.IncludeDefaultOperators) availableOperators = availableOperators.Where(o => !string.IsNullOrEmpty(o.Key));

            return availableOperators.FirstOrDefault(o => string.Equals(o.Token, name, StringComparison.InvariantCultureIgnoreCase));
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

        private Func<TClass, bool> Build(LogicalExpression source)
        {
            var factorized = FactorizeExpression(source);

            var p = Expression.Parameter(typeof(TClass));
            var body = Expression.Constant(true);
            foreach (var logicalExpression in factorized.Expressions.OfType<ILogicalExpression>()) //Or
            {
                foreach (var expression in logicalExpression.Expressions.OfType<RouteExpression>()) //And
                {
                    var complexExpression = expression as ComplexRouteExpression;
                    if (complexExpression != null)
                    {
                        if (!Map.IsValidComplexRoute(complexExpression.Route))
                            throw new ArgumentException($"Cannot process '{complexExpression.Route}' complex route");
                    }
                    else
                    {
                        var route = Map.GetRoute(expression.Route);
                        if (route == null)
                        {
                            var message = $"Cannot process '{expression.Route}' route";
                            throw new ArgumentException(message);
                        }

                        var @operator = GetOperator(route.Route, expression.Operator.Operator);
                        if (@operator == null)
                        {
                            var message = $"'{expression.Route}' route does not support '{expression.Operator.Operator}' operator";
                            throw new ArgumentException(message);
                        }

                        var accessor = ParameterReplacer.Replace(p, route.Accessor);
                        var stringValue = expression.Operator.Value.Trim('\"').Trim('\'');
                        var value = Converters.Convert(stringValue, route.Type);
                        var sss = @operator.ExpressionFactory(accessor, value);

                        //System.Linq.Expressions.Expression.AndAlso()
                        //body
                    }
                }
            }

            var lambda = Expression.Lambda(body, p);
            return (Func<TClass, bool>)lambda.Compile();
        }

        #endregion

        #region Nested type: ParameterReplacer

        class ParameterReplacer : ExpressionVisitor
        {
            #region Static members

            public static T Replace<T>(ParameterExpression parameter, T expression)
                where T : Expression
            {
                var replacer = new ParameterReplacer(parameter);
                return (T)replacer.Visit(expression);
            }

            #endregion

            private readonly ParameterExpression _expression;

            #region Constructors

            private ParameterReplacer(ParameterExpression expression)
            {
                if (expression == null) throw new ArgumentNullException(nameof(expression));
                _expression = expression;
            }

            #endregion

            #region Override members

            /// <summary>
            ///     Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.
            /// </summary>
            /// <returns>
            ///     The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
            /// </returns>
            /// <param name="node">The expression to visit.</param>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _expression;
            }

            #endregion
        }

        #endregion
    }
}