using PS.Query.Data.Predicate.Logic;

namespace PS.Query.Data.Predicate
{
    public interface IPredicateModelProvider
    {
        #region Members

        LogicalExpression Provide();

        #endregion
    }
}