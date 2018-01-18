using System;
using System.Linq.Expressions;

namespace PS.Query.Data.Predicate.Model
{
    public class PredicateOperator : Operator
    {
        #region Constructors

        public PredicateOperator()
        {
            ResultType = typeof(bool);
        }

        #endregion

        #region Properties

        public Func<Expression, Type, object, BinaryExpression> Expression { get; set; }

        #endregion
    }

    public class PredicateOperator<T> : PredicateOperator
    {
        #region Constructors

        public PredicateOperator()
        {
            SourceType = typeof(T);
        }

        #endregion
    }
}