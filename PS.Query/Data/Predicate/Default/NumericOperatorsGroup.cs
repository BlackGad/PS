using System.Linq.Expressions;
using PS.Data;
using PS.Query.Data.Predicate.Model;

namespace PS.Query.Data.Predicate.Default
{
    public class NumericOperatorsGroup<T> : DescriptorStorage<NumericOperatorsGroup<T>, Operator>
    {
        #region Static members

        public static PredicateOperator Equal
        {
            get
            {
                return FromCache(() => new PredicateOperator<T>
                {
                    Name = nameof(Equal),
                    Expression = (src, type, value) => Expression.Equal(src, Expression.Constant(value, type))
                });
            }
        }

        public static PredicateOperator Greater
        {
            get
            {
                return FromCache(() => new PredicateOperator<T>
                {
                    Name = nameof(Greater),
                    Expression = (src, type, value) => Expression.GreaterThan(src, Expression.Constant(value, type))
                });
            }
        }

        public static PredicateOperator GreaterOrEqual
        {
            get
            {
                return FromCache(() => new PredicateOperator<T>
                {
                    Name = nameof(GreaterOrEqual),
                    Expression = (src, type, value) => Expression.GreaterThanOrEqual(src, Expression.Constant(value, type))
                });
            }
        }

        public static PredicateOperator Less
        {
            get
            {
                return FromCache(() => new PredicateOperator<T>
                {
                    Name = nameof(Less),
                    Expression = (src, type, value) => Expression.LessThan(src, Expression.Constant(value, type))
                });
            }
        }

        public static PredicateOperator LessOrEqual
        {
            get
            {
                return FromCache(() => new PredicateOperator<T>
                {
                    Name = nameof(LessOrEqual),
                    Expression = (src, type, value) => Expression.LessThanOrEqual(src, Expression.Constant(value, type))
                });
            }
        }

        #endregion
    }
}