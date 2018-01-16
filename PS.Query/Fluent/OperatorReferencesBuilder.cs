using System;

namespace PS.Query.Fluent
{
    public class OperatorReferencesBuilder
    {
        private readonly SchemeRouteOptions _options;

        #region Constructors

        public OperatorReferencesBuilder(SchemeRouteOptions options)
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