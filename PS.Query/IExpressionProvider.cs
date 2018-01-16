using PS.Query.Model;

namespace PS.Query
{
    public interface IExpressionProvider
    {
        #region Members

        LogicalExpression Provide();

        #endregion
    }
}