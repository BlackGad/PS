using System.Collections.Generic;
using PS.Query.Fluent;

namespace PS.Query
{
    public class SchemeRouteOptions
    {
        #region Constructors

        public SchemeRouteOptions()
        {
            Operators = new OperatorReferencesBuilder(this);
            AdditionalOperators = new List<string>();
            IncludeDefaultOperators = true;
        }

        #endregion

        #region Properties

        public List<string> AdditionalOperators { get; }

        public bool IncludeDefaultOperators { get; set; }

        public OperatorReferencesBuilder Operators { get; }

        #endregion
    }
}