using System;
using System.Linq.Expressions;

namespace PS.Query.Configuration
{
    public class SimpleOperator : Operator
    {
        #region Properties

        public Func<Expression, object, BinaryExpression> ExpressionFactory { get; set; }

        #endregion
    }
}