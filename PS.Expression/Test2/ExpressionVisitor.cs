using System;

namespace PS.Expression.Test2
{
    public abstract class ExpressionVisitor<TClass>
    {
        #region Members

        public abstract Func<TClass, bool> Visit(ExpressionScheme<TClass> scheme);

        #endregion
    }
}