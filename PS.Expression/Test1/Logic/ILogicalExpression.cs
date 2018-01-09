using System.Collections;

namespace PS.Expression.Test1.Logic
{
    public interface ILogicalExpression
    {
        #region Properties

        IEnumerable Expressions { get; }
        LogicalOperator Operator { get; }

        #endregion

        #region Members

        void AddExpression(object expression);
        void RemoveExpression(object expression);

        #endregion
    }
}