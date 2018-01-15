using System.Collections.Generic;
using PS.Query.Fluent;

namespace PS.Query
{
    public class ExpressionSchemeRouteOptions
    {
        #region Constructors

        public ExpressionSchemeRouteOptions()
        {
            Operators = new ExpressionOperatorReferencesBuilder(this);
            AdditionalOperators = new List<string>();
            IncludeDefaultOperators = true;
        }

        #endregion

        #region Properties

        public List<string> AdditionalOperators { get; }

        public bool IncludeDefaultOperators { get; set; }

        public ExpressionOperatorReferencesBuilder Operators { get; }

        #endregion
    }
}