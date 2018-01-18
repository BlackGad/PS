using System;
using PS.Data.Predicate.ExpressionBuilder;

namespace PS.Data.Predicate
{
    public static class Scheme
    {
        #region Static members

        public static IScheme<TClass> Create<TClass>()
        {
            return new Scheme<TClass>();
        }

        #endregion
    }

    internal class Scheme<TClass> : IPredicateSchemeProvider,
                                    IScheme<TClass>
    {
        private readonly PredicateConverters _converters;
        private readonly PredicateOperators _operators;

        private readonly PredicateRoutes<TClass> _routes;

        #region Constructors

        public Scheme()
        {
            _routes = new PredicateRoutes<TClass>();
            _converters = new PredicateConverters();
            _operators = new PredicateOperators();
        }

        #endregion

        #region IScheme<TClass> Members

        IPredicateConverters IScheme<TClass>.Converters
        {
            get { return _converters; }
        }

        IPredicateOperators IScheme<TClass>.Operators
        {
            get { return _operators; }
        }

        IPredicateRoutes<TClass> IScheme<TClass>.Routes
        {
            get { return _routes; }
        }

        public Func<TClass, bool> Build(IPredicateModelProvider provider)
        {
            return PredicateExpressionBuilder.Build<TClass>(this, provider);
        }

        #endregion

        #region IPredicateSchemeProvider Members

        IPredicateOperatorsProvider IPredicateSchemeProvider.Operators
        {
            get { return _operators; }
        }

        IPredicateRoutesProvider IPredicateSchemeProvider.Routes
        {
            get { return _routes; }
        }

        IPredicateConvertersProvider IPredicateSchemeProvider.Converters
        {
            get { return _converters; }
        }

        #endregion
    }
}