using System;
using System.Linq.Expressions;
using System.Reflection;
using PS.Data;
using PS.Query.Data.Predicate.Model;

namespace PS.Query.Data.Predicate.Default
{
    public class StringOperatorsGroup : DescriptorStorage<StringOperatorsGroup, Operator>
    {
        #region Constants

        private static readonly MethodInfo StringContainsMethod;
        private static readonly MethodInfo StringEndWithMethod;
        private static readonly MethodInfo StringEqualsMethod;
        private static readonly MethodInfo StringStartWithMethod;

        #endregion

        #region Static members

        private static BinaryExpression BuildExpression(object value, Expression src, Type type, MethodInfo method)
        {
            var constant = Expression.Constant(value, type);
            BinaryExpression result;
            if (value == null) result = Expression.Equal(src, constant);
            else
            {
                result = Expression.NotEqual(src, Expression.Constant(null, type));
                result = Expression.AndAlso(result, Expression.Call(src, method, constant));
            }
            return result;
        }

        public static PredicateOperator Contains
        {
            get
            {
                return FromCache(() => new PredicateOperator<string>
                {
                    Name = nameof(Contains),
                    Expression = (src, type, value) => BuildExpression(value, src, type, StringContainsMethod)
                });
            }
        }

        public static PredicateOperator EndWith
        {
            get
            {
                return FromCache(() => new PredicateOperator<string>
                {
                    Name = nameof(EndWith),
                    Expression = (src, type, value) => BuildExpression(value, src, type, StringEndWithMethod)
                });
            }
        }

        public static PredicateOperator Equal
        {
            get
            {
                return FromCache(() => new PredicateOperator<string>
                {
                    Name = nameof(Equal),
                    Expression = (src, type, value) => BuildExpression(value, src, type, StringEqualsMethod)
                });
            }
        }

        public static PredicateOperator StartWith
        {
            get
            {
                return FromCache(() => new PredicateOperator<string>
                {
                    Name = nameof(StartWith),
                    Expression = (src, type, value) => BuildExpression(value, src, type, StringStartWithMethod)
                });
            }
        }

        #endregion

        #region Constructors

        static StringOperatorsGroup()
        {
            StringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            StringEqualsMethod = typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string) });
            StringStartWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) });
            StringEndWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) });
        }

        #endregion
    }
}