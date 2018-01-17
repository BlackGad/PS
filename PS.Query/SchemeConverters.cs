using System;
using System.Collections.Generic;

namespace PS.Query
{
    internal class SchemeConverters : ISchemeConverterBuilder,
                                      ISchemeConverterProvider
    {
        readonly Dictionary<Type, Func<string, object>> _converters;

        #region Constructors

        public SchemeConverters()
        {
            _converters = new Dictionary<Type, Func<string, object>>();
        }

        #endregion

        #region ISchemeConverterBuilder Members

        public ISchemeConverterBuilder Register<T>(Func<string, T> converter)
        {
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            _converters.Add(typeof(T), s => converter(s));
            return this;
        }

        #endregion

        #region ISchemeConverterProvider Members

        public object Convert(string value, Type type)
        {
            if (!_converters.ContainsKey(type)) throw new InvalidCastException($"Converter to {type} not defined");
            return _converters[type](value);
        }

        #endregion
    }
}