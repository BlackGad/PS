using PS.Data.Predicate.Logic;

namespace PS.Data.Predicate
{
    public interface IPredicateModelProvider
    {
        #region Members

        LogicalExpression Provide();

        #endregion
    }
}