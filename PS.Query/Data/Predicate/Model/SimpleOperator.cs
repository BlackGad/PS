using System;
using System.Linq.Expressions;

namespace PS.Query.Data.Predicate.Model
{
    public class SimpleOperator : Operator
    {
        #region Properties

        public Func<Expression, object, BinaryExpression> ExpressionFactory { get; set; }

        #endregion
    }
}