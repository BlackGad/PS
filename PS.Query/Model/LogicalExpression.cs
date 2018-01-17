using System;
using System.Collections;
using System.Linq;
using PS.Data.Logic;

namespace PS.Query.Model
{
    public class LogicalExpression : ILogicalExpression
    {
        #region Constructors

        public LogicalExpression(LogicalOperator op, RouteExpression[] expressions)
        {
            Expressions = expressions;
            Operator = op;
        }

        #endregion

        #region Properties

        public RouteExpression[] Expressions { get; }
        public LogicalOperator Operator { get; }

        IEnumerable ILogicalExpression.Expressions
        {
            get { return Expressions; }
        }

        #endregion

        #region Override members

        public override string ToString()
        {
            return string.Join($" {Operator.ToString().ToUpperInvariant()} ", Expressions.Select(o => "(" + o.ToString() + ")"));
        }

        #endregion

        #region ILogicalExpression Members

        void ILogicalExpression.AddExpression(object expression)
        {
            throw new NotSupportedException();
        }

        void ILogicalExpression.RemoveExpression(object expression)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}