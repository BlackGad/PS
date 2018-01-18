using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PS.Extensions
{
    public static class StringExtensions
    {
        #region Static members

        /// <summary>
        ///     Converts string value to primitive type.
        /// </summary>
        /// <param name="stringValue">String value which will be converted.</param>
        /// <param name="type">Primitive type. Possible types can be get calling ObjectExtensions.GetPrimitiveTypes()</param>
        /// <returns>Converted string value. If string value is null or empty default primitive value will be returned.</returns>
        /// <exception cref="System.NotSupportedException">
        ///     If type is not primitive or primitive type has no public static Parse
        ///     method exception will be throw.
        /// </exception>
        public static object ConvertToPrimitive(this string stringValue, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (!ObjectExtensions.GetPrimitiveTypes().Contains(type)) throw new NotSupportedException($"{type} type is not supported");
            if (string.IsNullOrEmpty(stringValue)) return Activator.CreateInstance(type);
            if (type == typeof(string)) return stringValue;

            var parseMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "Parse").ToList();
            var parseWithCultureMethod = parseMethods.FirstOrDefault(m =>
            {
                var args = m.GetParameters();
                return args.Length == 2 && args[0].ParameterType == typeof(string) && args[1].ParameterType == typeof(IFormatProvider);
            });
            if (parseWithCultureMethod != null)
                return parseWithCultureMethod.Invoke(null, new object[] { stringValue, CultureInfo.InvariantCulture });

            var parseMethod = parseMethods.FirstOrDefault(m =>
            {
                var args = m.GetParameters();
                return args.Length == 1 && args[0].ParameterType == typeof(string);
            });
            if (parseMethod != null) return parseMethod.Invoke(null, new object[] { stringValue });
            throw new NotSupportedException();
        }

        public static int Occurrences(this string input, char value)
        {
            var count = 0;
            for (var index = 0; index < input.Length; index++)
            {
                if (input[index] == value) count ++;
            }
            return count;
        }

        #endregion
    }
}