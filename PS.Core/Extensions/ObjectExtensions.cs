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

        public static int MergeHash(this int hash, int addHash)
        {
            return (hash*397) ^ addHash;
        }

        #endregion
    }
}