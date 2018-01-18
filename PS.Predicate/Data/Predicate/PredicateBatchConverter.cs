using System;

namespace PS.Data.Predicate
{
    public class PredicateBatchConverter
    {
        #region Constructors

        public PredicateBatchConverter(Predicate<Type> predicate, Func<Type, string, object> converter)
        {
            Predicate = predicate;
            Converter = converter;
        }

        #endregion

        #region Properties

        public Func<Type, string, object> Converter { get; }

        public Predicate<Type> Predicate { get; }

        #endregion
    }
}