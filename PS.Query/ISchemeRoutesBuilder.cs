using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PS.Navigation;

namespace PS.Query
{
    public interface ISchemeRoutesBuilder<TClass>
    {
        #region Members

        ISchemeRoutesBuilder<TResult> Complex<TResult>(Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                       Action<SchemeRouteOptions> options = null);

        ISchemeRoutesBuilder<TResult> Complex<TResult>(Route route,
                                                       Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                       Action<SchemeRouteOptions> options = null);

        ISchemeRoutesBuilder<TClass> Route<TResult>(Expression<Func<TClass, TResult>> accessor,
                                                    Action<SchemeRouteOptions> options = null);

        ISchemeRoutesBuilder<TClass> Route<TResult>(Route route,
                                                    Expression<Func<TClass, TResult>> accessor,
                                                    Action<SchemeRouteOptions> options = null);

        #endregion
    }
}