using System;

namespace PS.Query.Data.Predicate.Model
{
    public abstract class Operator
    {
        #region Properties

        public string Key { get; set; }

        public string Name { get; set; }

        public Type ResultType { get; set; }

        public Type SourceType { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            return $"{Name} ( {SourceType} ): {ResultType}";
        }

        #endregion
    }
}