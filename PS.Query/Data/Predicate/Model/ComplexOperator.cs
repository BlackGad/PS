using System;
using System.Linq.Expressions;

namespace PS.Query.Data.Predicate.Model
{
    public class ComplexOperator : Operator
    {
        #region Properties

        public Func<Expression, LambdaExpression, Expression> ExpressionFactory { get; set; }

        #endregion
    }
}