using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PS.Data.Predicate.Model;

namespace PS.Data.Predicate.Default
{
    public class SubsetOperatorsGroup : DescriptorStorage<SubsetOperatorsGroup, Operator>
    {
        #region Constants

        private static readonly MethodInfo EnumerableGenericAnyMethod;
        private static readonly MethodInfo EnumerableGenericCountMethod;
        private static readonly MethodInfo EnumerableGenericWhereMethod;

        #endregion

        #region Static members

        private static Expression BuildAnyExpression(Expression src, LambdaExpression sub)
        {
            var sourceType = src.Type;
            var itemsType = sourceType.IsGenericType ? sourceType.GetGenericArguments()[0] : typeof(object);
            var anyMethod = EnumerableGenericAnyMethod.MakeGenericMethod(itemsType);

            Expression anyCall = Expression.Call(anyMethod, src, sub);

            return anyCall;
        }

        private static Expression BuildCountExpression(Expression src, LambdaExpression sub)
        {
            var sourceType = src.Type;
            var itemsType = sourceType.IsGenericType ? sourceType.GetGenericArguments()[0] : typeof(object);

            var whereMethod = EnumerableGenericWhereMethod.MakeGenericMethod(itemsType);
            var countMethod = EnumerableGenericCountMethod.MakeGenericMethod(itemsType);

            Expression whereCall = Expression.Call(whereMethod, src, sub);
            Expression countCall = Expression.Call(countMethod, whereCall);

            return countCall;
        }

        public static SubsetOperator Any
        {
            get
            {
                return FromCache(() => new SubsetOperator<bool>
                {
                    Name = nameof(Any),
                    Expression = BuildAnyExpression
                });
            }
        }

        public static SubsetOperator Count
        {
            get
            {
                return FromCache(() => new SubsetOperator<int>
                {
                    Name = nameof(Count),
                    Expression = BuildCountExpression
                });
            }
        }

        #endregion

        #region Constructors

        static SubsetOperatorsGroup()
        {
            EnumerableGenericWhereMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == nameof(Enumerable.Where) && m.GetParameters().Length == 2);

            EnumerableGenericCountMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == nameof(Enumerable.Count) && m.GetParameters().Length == 1);

            EnumerableGenericAnyMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2);
        }

        #endregion
    }
}