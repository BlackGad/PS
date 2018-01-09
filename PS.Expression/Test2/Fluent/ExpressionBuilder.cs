using System;

namespace PS.Expression.Test2.Fluent
{
    public class ExpressionBuilder<TClass>
    {
        private readonly ExpressionScheme<TClass> _scheme;

        #region Constructors

        public ExpressionBuilder(ExpressionScheme<TClass> scheme)
        {
            if (scheme == null) throw new ArgumentNullException(nameof(scheme));
            _scheme = scheme;
        }

        #endregion

        #region Members

        public Func<TClass, bool> Build()
        {
            return null;
        }

        #endregion
    }
}