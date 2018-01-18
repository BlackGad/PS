using PS.Query.Data.Predicate.Model;

namespace PS.Query.Data.Predicate
{
    public interface IPredicateOperators
    {
        #region Members

        ComplexOperatorBuilder<TResult> Complex<TResult>(string token);
        IPredicateOperators Register(Operator op);
        SimpleOperatorBuilder<TSource> Simple<TSource>(string token);

        #endregion
    }
}