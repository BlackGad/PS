namespace PS.Query.Data.Predicate
{
    public interface IPredicateConverters
    {
        #region Members

        IPredicateConverters Register(PredicateBatchConverter converter);

        IPredicateConverters Reset();

        #endregion
    }
}