using System;
using System.Collections.Generic;

namespace PS.Extensions
{
    public static class TypeExtensions
    {
        #region Static members

        /// <summary>
        ///     Gets linear type hierarchy
        /// </summary>
        /// <param name="type">Source type</param>
        /// <param name="predicate">Enumerate filter predicate</param>
        /// <returns>Linear type hierarchy</returns>
        public static IEnumerable<Type> EnumerateHierarchy(this Type type, Func<Type, bool> predicate = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var currentType = type;

            while (true)
            {
                if (currentType == null) break;
                if (predicate?.Invoke(currentType) == false) break;
                yield return currentType;
                currentType = currentType.BaseType;
            }
        }

        #endregion
    }
}