using System.Linq.Expressions;
using PS.Data.Predicate.Model;

namespace PS.Data.Predicate.Default
{
    public class EqualOperatorsGroup<T> : DescriptorStorage<NumericOperatorsGroup<T>, Operator>
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

        #endregion
    }
}