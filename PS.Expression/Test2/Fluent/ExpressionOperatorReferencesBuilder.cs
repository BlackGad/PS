using System;

namespace PS.Expression.Test2.Fluent
{
    public class ExpressionOperatorReferencesBuilder
    {
        private readonly ExpressionSchemeRouteOptions _options;

        #region Constructors

        public ExpressionOperatorReferencesBuilder(ExpressionSchemeRouteOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _options = options;
        }

        #endregion

        #region Members

        public ExpressionOperatorReferencesBuilder Include(string key)
        {
            _options.AdditionalOperators.Add(key);
            return this;
        }

        public ExpressionOperatorReferencesBuilder Reset()
        {
            _options.IncludeDefaultOperators = false;
            return this;
        }

        #endregion
    }
}