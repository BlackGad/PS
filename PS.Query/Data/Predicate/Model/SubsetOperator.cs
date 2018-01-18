using System;
using System.Collections;
using System.Linq.Expressions;

namespace PS.Query.Data.Predicate.Model
{
    public class SubsetOperator : Operator
    {
        #region Constructors

        public SubsetOperator()
        {
            SourceType = typeof(IEnumerable);
        }

        #endregion

        #region Properties

        public Func<Expression, LambdaExpression, Expression> Expression { get; set; }

        #endregion
    }

    public class SubsetOperator<T> : SubsetOperator
    {
        #region Constructors

        public SubsetOperator()
        {
            ResultType = typeof(T);
        }

        #endregion
    }
}