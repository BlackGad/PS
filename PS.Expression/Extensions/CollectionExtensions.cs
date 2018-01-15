﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PS.Query.Extensions
{
    public static class CollectionExtensions
    {
        #region Static members

        /// <summary>
        ///     Enumerates <paramref name="object" /> as generic enumeration and cast it to target type.
        /// </summary>
        /// <param name="object">Source object.</param>
        /// <returns>
        ///     If <paramref name="object" /> is <see cref="IEnumerable" /> and enumeration is null returns empty enumeration,
        ///     source enumeration otherwise.
        /// </returns>
        public static IEnumerable<T> Enumerate<T>(this object @object)
        {
            var enumerable = @object as IEnumerable;
            return enumerable?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        /// <summary>
        ///     Enumerates enumeration.
        /// </summary>
        /// <param name="enumerable">IEnumerable object.</param>
        /// <returns>If enumeration is null returns empty enumeration, source enumeration otherwise.</returns>
        public static IEnumerable Enumerate(this IEnumerable enumerable)
        {
            return enumerable ?? Enumerable.Empty<object>();
        }

        /// <summary>
        ///     Enumerates enumeration.
        /// </summary>
        /// <param name="enumerable">IEnumerable object.</param>
        /// <returns>If enumeration is null returns empty enumeration, source enumeration otherwise.</returns>
        public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        /// <summary>
        ///     Enumerates <paramref name="object" /> as generic enumeration.
        /// </summary>
        /// <param name="object">Source object.</param>
        /// <returns>
        ///     If <paramref name="object" /> is <see cref="IEnumerable" /> and enumeration is null returns empty enumeration,
        ///     source enumeration otherwise.
        /// </returns>
        public static IEnumerable<object> Enumerate(this object @object)
        {
            var enumerable = @object as IEnumerable;
            return enumerable.Enumerate<object>();
        }

        #endregion
    }
}