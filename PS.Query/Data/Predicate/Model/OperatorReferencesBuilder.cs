using System;

namespace PS.Query.Data.Predicate.Model
{
    public class OperatorReferencesBuilder
    {
        private readonly PredicateRouteOptions _options;

        #region Constructors

        public OperatorReferencesBuilder(PredicateRouteOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _options = options;
        }

        #endregion

        #region Members

        public OperatorReferencesBuilder Include(string key)
        {
            _options.AdditionalOperators.Add(key);
            return this;
        }

        public OperatorReferencesBuilder Reset()
        {
            _options.IncludeDefaultOperators = false;
            return this;
        }

        #endregion
    }
}