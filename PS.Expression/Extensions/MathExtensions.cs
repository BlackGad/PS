using System;
using System.Linq;
using PS.Expression.Logic;

namespace PS.Expression.Extensions
{
    public static class MathExtensions
    {
        #region Static members

        public static ILogicalExpression Factorize(this object expression, FactorizeParams factorizeParams)
        {
            if (factorizeParams == null) throw new ArgumentNullException(nameof(factorizeParams));
            if (factorizeParams.CompositionFactory == null) throw new ArgumentNullException(nameof(factorizeParams));
            if (factorizeParams.ConvertFactory == null) throw new ArgumentNullException(nameof(factorizeParams));

            return Factorize(expression, factorizeParams, false);
        }

        private static ILogicalExpression Factorize(object expression, FactorizeParams factorizeParams, bool inverted)
        {
            var rootOrExpression = factorizeParams.CompositionFactory(LogicalOperator.Or);
            expression = factorizeParams.ConvertFactory(expression, inverted);

            var invertedExpression = expression as IInvertedLogicalExpression;
            if (invertedExpression != null) expression = Factorize(invertedExpression.Expression, factorizeParams, !inverted);

            var compositionExpression = expression as ILogicalExpression;
            if (compositionExpression != null)
            {
                var subExpressions = compositionExpression.Expressions ?? Enumerable.Empty<object>();
                foreach (var subExpression in subExpressions)
                {
                    var factorizedSubExpression = Factorize(subExpression, factorizeParams, inverted);
                    switch (compositionExpression.Operator)
                    {
                        case LogicalOperator.And:
                            MultiplyFactorizedExpression(rootOrExpression, factorizedSubExpression.Expressions.Enumerate<object>().ToArray());
                            break;
                        case LogicalOperator.Or:
                            foreach (var sub in factorizedSubExpression.Expressions)
                            {
                                rootOrExpression.AddExpression(sub);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (!rootOrExpression.Expressions.Enumerate<object>().Any())
            {
                var rootAndExpression = factorizeParams.CompositionFactory(LogicalOperator.And);
                rootOrExpression.AddExpression(rootAndExpression);
            }

            if (compositionExpression == null && expression != null)
                rootOrExpression.Expressions.Enumerate().OfType<ILogicalExpression>().First().AddExpression(expression);

            return rootOrExpression;
        }

        private static void MultiplyFactorizedExpression(ILogicalExpression targetLogical, object[] source)
        {
            if (targetLogical.Expressions.Enumerate<object>().Any())
            {
                foreach (var targetSubGroup in targetLogical.Expressions.OfType<ILogicalExpression>())
                {
                    foreach (var sourceSubGroup in source.OfType<ILogicalExpression>())
                    {
                        foreach (var expression in sourceSubGroup.Expressions)
                        {
                            targetSubGroup.AddExpression(expression);
                        }
                    }
                }
            }
            else
            {
                foreach (var expression in source)
                {
                    targetLogical.AddExpression(expression);
                }
            }
        }

        #endregion
    }
}