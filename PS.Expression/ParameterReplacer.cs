using System;
using System.Linq.Expressions;

namespace PS.Query
{
    public class ParameterReplacer : ExpressionVisitor
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
}