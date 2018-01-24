using System;
using System.Linq;
using PS.Extensions;

namespace PS.Data.Logic.Extensions
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

            var logicalExpression = expression as ILogicalExpression;
            if (logicalExpression != null)
            {
                var subExpressions = logicalExpression.Expressions.Enumerate<object>();
                foreach (var subExpression in subExpressions)
                {
                    var factorizedSubExpression = Factorize(subExpression, factorizeParams, inverted);
                    switch (logicalExpression.Operator)
                    {
                        case LogicalOperator.And:
                            MultiplyFactorizedExpressions(rootOrExpression,
                                                          factorizedSubExpression,
                                                          factorizeParams);
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

            if (logicalExpression == null && expression != null)
                rootOrExpression.Expressions.Enumerate().OfType<ILogicalExpression>().First().AddExpression(expression);

            return rootOrExpression;
        }

        private static void MultiplyFactorizedExpressions(ILogicalExpression orExpression,
                                                          ILogicalExpression factorizedExpression,
                                                          FactorizeParams factorizeParams)
        {
            if (orExpression.Expressions.Enumerate<object>().Any())
            {
                var orExpressions = orExpression.Expressions.OfType<ILogicalExpression>().ToList();
                foreach (var oldAndGroupExpression in orExpressions)
                {
                    orExpression.RemoveExpression(oldAndGroupExpression);
                    foreach (var multipliedOrExpression in factorizedExpression.Expressions.OfType<ILogicalExpression>()) //or
                    {
                        var newAndGroupExpression = factorizeParams.CompositionFactory(LogicalOperator.And);
                        foreach (var expression in oldAndGroupExpression.Expressions)
                        {
                            newAndGroupExpression.AddExpression(expression);
                        }

                        foreach (var expression in multipliedOrExpression.Expressions) //and
                        {
                            newAndGroupExpression.AddExpression(expression);
                        }

                        orExpression.AddExpression(newAndGroupExpression);
                    }
                }
            }
            else
            {
                foreach (var expression in factorizedExpression.Expressions)
                {
                    orExpression.AddExpression(expression);
                }
            }
        }

        #endregion
    }
}