using System;
using System.Collections.Generic;
using System.Linq;
using PS.Query.Data.Predicate.Default;
using PS.Query.Data.Predicate.ExpressionBuilder;

namespace PS.Query.Data.Predicate
{
    internal class PredicateConverters : IPredicateConverters,
                                         IPredicateConvertersProvider
    {
        readonly IList<PredicateBatchConverter> _converters;

        #region Constructors

        public PredicateConverters()
        {
            _converters = new List<PredicateBatchConverter>(Converters.All);
        }

        #endregion

        #region IPredicateConverters Members

        public IPredicateConverters Register(PredicateBatchConverter converter)
        {
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            _converters.Add(converter);
            return this;
        }

        public IPredicateConverters Reset()
        {
            _converters.Clear();
            return this;
        }

        #endregion

        #region IPredicateConvertersProvider Members

        public object Convert(string value, Type type)
        {
            var converter = _converters.Reverse().FirstOrDefault(c => c.Predicate(type));
            if (converter != null) return converter.Converter(type, value);

            throw new InvalidCastException($"Cannot convert {value} to {type} type");
        }

        #endregion
    }
}