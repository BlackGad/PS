using System;
using PS.Data.Predicate.Model;

namespace PS.Data.Predicate.Extensions
{
    public static class PredicateOperatorsExtensions
    {
        #region Static members

        public static PredicateOperatorBuilder<TSource> Predicate<TSource>(this IPredicateOperators predicateOperators, string token)
        {
            if (predicateOperators == null) throw new ArgumentNullException(nameof(predicateOperators));
            if (token == null) throw new ArgumentNullException(nameof(token));
            return new PredicateOperatorBuilder<TSource>(predicateOperators, token);
        }

        public static SubsetOperatorBuilder<TResult> Subset<TResult>(this IPredicateOperators predicateOperators, string token)
        {
            if (predicateOperators == null) throw new ArgumentNullException(nameof(predicateOperators));
            if (token == null) throw new ArgumentNullException(nameof(token));
            return new SubsetOperatorBuilder<TResult>(predicateOperators, token);
        }

        #endregion
    }
}