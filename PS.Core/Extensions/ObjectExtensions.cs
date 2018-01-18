using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PS.Extensions
{
    public static class ObjectExtensions
    {
        #region Static members

        public static T Create<T>() where T : new() => new T();

        public static IEnumerable<object> Enumerate(this object obj)
        {
            return obj.Enumerate<object>();
        }

        public static IEnumerable<T> Enumerate<T>(this object obj)
        {
            var enumerable = obj as IEnumerable;
            return enumerable?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> obj)
        {
            var enumerable = obj as IEnumerable;
            return enumerable?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        /// <summary>
        ///     Returns all well-known primitive types
        /// </summary>
        /// <returns>Collection of primitive types</returns>
        public static IReadOnlyCollection<Type> GetPrimitiveTypes()
        {
            return new List<Type>
            {
                typeof(String),
                typeof(Boolean),
                typeof(Char),
                typeof(SByte),
                typeof(Byte),
                typeof(Int16),
                typeof(UInt16),
                typeof(Int32),
                typeof(UInt32),
                typeof(Int64),
                typeof(UInt64),
                typeof(Single),
                typeof(Double),
                typeof(Decimal),
                typeof(Guid)
            };
        }

        public static int MergeHash(this int hash, int addHash)
        {
            return (hash*397) ^ addHash;
        }

        #endregion
    }
}