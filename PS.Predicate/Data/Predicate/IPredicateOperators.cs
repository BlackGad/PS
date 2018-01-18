using PS.Data.Predicate.Model;

namespace PS.Data.Predicate
{
    public interface IPredicateOperators
    {
        #region Members

        IPredicateOperators Register(Operator op);

        IPredicateOperators Reset();

        #endregion
    }
}