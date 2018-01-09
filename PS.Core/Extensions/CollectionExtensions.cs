using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PS.Extensions
{
    public static class CollectionExtensions
    {
        #region Static members

        public static TValue Ensure<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, FastActivator.Create<TValue>());
            return dictionary[key];
        }

        public static TValue Ensure<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, factory());
            return dictionary[key];
        }

        /// <summary>
        ///     Convert IEnumerable source to the ManagedList.
        /// </summary>
        /// <remarks>
        ///     The value must be IEnumerable.
        /// </remarks>
        /// <param name="enumeration">
        ///     IEnumerable source which we want to convert.
        /// </param>
        /// <param name="keySelector">Specified key selector</param>
        /// <param name="elementSelector">Specified value selector</param>
        /// <param name="generateExceptionOnError">Generate exception on add failure</param>
        /// <returns>
        ///     ConcurrentDictionary with key type CreateTKey and value type TSource.
        /// </returns>
        /// <permission>
        ///     Everyone can access this method.
        /// </permission>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> enumeration,
                                                                                                           Func<TSource, TKey> keySelector,
                                                                                                           Func<TSource, TElement> elementSelector,
                                                                                                           bool generateExceptionOnError = false)
        {
            if (enumeration == null) throw new ArgumentNullException(nameof(enumeration));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

            var result = new ConcurrentDictionary<TKey, TElement>();
            foreach (TSource element in enumeration)
            {
                var operationResult = result.TryAdd(keySelector(element), elementSelector(element));
                if (generateExceptionOnError && !operationResult) throw new ArgumentException("An item with the same key has already been added.");
            }
            return result;
        }

        /// <summary>
        ///     Convert <see cref="IEnumerable" /> source to the <see cref="ConcurrentDictionary`2" />.
        /// </summary>
        /// <remarks>
        ///     The value must be <see cref="IEnumerable" />.
        /// </remarks>
        /// <param name="enumeration">
        ///     <see cref="IEnumerable" /> source which we want to convert.
        /// </param>
        /// <param name="keySelector">Specified key selector</param>
        /// <param name="generateExceptionOnError">Generate exception on add failure</param>
        /// <returns>
        ///     <see cref="ConcurrentDictionary`2" /> with key type <typeparamref name="TKey" /> and value type TSource.
        /// </returns>
        /// <permission>
        ///     Everyone can access <c>this</c> method.
        /// </permission>
        public static ConcurrentDictionary<TKey, TSource> ToConcurrentDictionary<TSource, TKey>(this IEnumerable<TSource> enumeration,
                                                                                                Func<TSource, TKey> keySelector,
                                                                                                bool generateExceptionOnError = false)
        {
            return ToConcurrentDictionary(enumeration, keySelector, s => s, generateExceptionOnError);
        }

        #endregion
    }
}