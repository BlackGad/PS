using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PS.Expression.Test2
{
    public class ExpressionSchemeProperties<TClass>
    {
        #region Members

        public ExpressionSchemeProperties<TResult> Subset<TResult>(Expression<Func<TClass, IEnumerable<TResult>>> instruction,
                                                                      Action<ExpressionSchemePropertyOptions> options = null)
        {
            return new ExpressionSchemeProperties<TResult>();
        }

        public ExpressionSchemeProperties<TClass> Path<TResult>(Expression<Func<TClass, TResult>> instruction,
                                                                  Action<ExpressionSchemePropertyOptions> options = null)
        {
            return this;
        }

        #endregion
    }
}