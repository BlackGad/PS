using System.Collections.Generic;
using PS.Query.Data.Predicate.Model;

namespace PS.Query.Data.Predicate
{
    public class PredicateRouteOptions
    {
        #region Constructors

        public PredicateRouteOptions()
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