using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PS.Expression.Logic
{
    public class LogicalExpression : ILogicalExpression
    {
        private readonly List<object> _expressions;

        #region Constructors

        public LogicalExpression(LogicalOperator op)
        {
            _expressions = new List<object>();
            Operator = op;
        }

        #endregion

        #region Properties

        public IEnumerable Expressions
        {
            get { return _expressions; }
        }

        public LogicalOperator Operator { get; }

        #endregion

        #region Override members

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Join($" {Operator.ToString().ToUpperInvariant()} ", _expressions.Select(o => "(" + o.ToString() + ")"));
        }

        #endregion

        #region ILogicalExpression Members

        public void AddExpression(object expression)
        {
            _expressions.Add(expression);
        }

        public void RemoveExpression(object expression)
        {
            _expressions.Remove(expression);
        }

        #endregion
    }
}