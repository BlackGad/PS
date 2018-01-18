using System;

namespace PS.Query.Data.Predicate.Extensions
{
    public static class PredicateConvertersExtensions
    {
        #region Static members

        public static IPredicateConverters Register<T>(this IPredicateConverters predicateConverters, Func<string, T> converter, bool direct = true)
        {
            if (predicateConverters == null) throw new ArgumentNullException(nameof(predicateConverters));
            if (converter == null) throw new ArgumentNullException(nameof(converter));

            var predicate = direct
                ? new Predicate<Type>(type => type == typeof(T))
                : (type => typeof(T).IsAssignableFrom(type));

            predicateConverters.Register(new PredicateBatchConverter(predicate, (type, s) => converter(s)));
            return predicateConverters;
        }

        public static IPredicateConverters Register(this IPredicateConverters predicateConverters,
                                                    Predicate<Type> predicate,
                                                    Func<Type, string, object> converter)
        {
            if (predicateConverters == null) throw new ArgumentNullException(nameof(predicateConverters));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            predicateConverters.Register(new PredicateBatchConverter(predicate, converter));
            return predicateConverters;
        }

        #endregion
    }
}