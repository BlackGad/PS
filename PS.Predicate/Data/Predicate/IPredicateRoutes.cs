using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PS.Navigation;

namespace PS.Data.Predicate
{
    public interface IPredicateRoutes<TClass>
    {
        #region Members

        IPredicateRoutes<TClass> Route<TResult>(Route route,
                                                Expression<Func<TClass, TResult>> accessor,
                                                Action<PredicateRouteOptions> options = null);

        IPredicateRoutes<TResult> Subset<TResult>(Route route,
                                                  Expression<Func<TClass, IEnumerable<TResult>>> accessor,
                                                  Action<PredicateRouteOptions> options = null);

        #endregion
    }
}