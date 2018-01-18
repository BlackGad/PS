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

        public OperatorReferencesBuilder ExcludeDefault(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _options.ExcludeDefaultOperators.Add(name);
            return this;
        }

        public OperatorReferencesBuilder Include(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
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