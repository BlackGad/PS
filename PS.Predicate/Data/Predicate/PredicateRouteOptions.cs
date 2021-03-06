using System.Collections.Generic;
using PS.Data.Predicate.Model;

namespace PS.Data.Predicate
{
    public class PredicateRouteOptions
    {
        #region Constructors

        public PredicateRouteOptions()
        {
            Operators = new OperatorReferencesBuilder(this);
            AdditionalOperators = new List<string>();
            ExcludeDefaultOperators = new List<string>();
            IncludeDefaultOperators = true;
        }

        #endregion

        #region Properties

        public List<string> AdditionalOperators { get; }
        public List<string> ExcludeDefaultOperators { get; }

        public bool IncludeDefaultOperators { get; set; }

        public OperatorReferencesBuilder Operators { get; }

        #endregion
    }
}