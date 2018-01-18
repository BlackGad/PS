using PS.Query.Data.Predicate.Model;

namespace PS.Query.Data.Predicate
{
    public interface IPredicateOperators
    {
        #region Members

        IPredicateOperators Register(Operator op);

        IPredicateOperators Reset();

        #endregion
    }
}