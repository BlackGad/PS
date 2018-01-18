﻿using System;

namespace PS.Query.Data.Predicate
{
    public interface IScheme<T>
    {
        #region Properties

        IPredicateConverters Converters { get; }

        IPredicateOperators Operators { get; }

        IPredicateRoutes<T> Routes { get; }

        #endregion

        #region Members

        Func<T, bool> Build(IPredicateModelProvider provider);

        #endregion
    }
}