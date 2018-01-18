namespace PS.Data.Predicate
{
    public interface IPredicateConverters
    {
        #region Members

        IPredicateConverters Register(PredicateBatchConverter converter);

        IPredicateConverters Reset();

        #endregion
    }
}