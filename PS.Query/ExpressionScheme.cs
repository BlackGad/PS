using System;

namespace PS.Query
{
    public static class ExpressionScheme
    {
        #region Static members

        public static IExpressionSchemeBuilder<TClass> Create<TClass>()
        {
            return new ExpressionScheme<TClass>();
        }

        #endregion
    }

    internal class ExpressionScheme<TClass> : IExpressionSchemeProvider,
                                              IExpressionSchemeBuilder<TClass>
    {
        private readonly SchemeConverters _converters;
        private readonly SchemeOperators _operators;

        private readonly SchemeRoutes<TClass> _routes;

        #region Constructors

        public ExpressionScheme()
        {
            _routes = new SchemeRoutes<TClass>();
            _converters = new SchemeConverters();
            _operators = new SchemeOperators();
        }

        #endregion

        #region IExpressionSchemeBuilder<TClass> Members

        ISchemeConverterBuilder IExpressionSchemeBuilder<TClass>.Converters
        {
            get { return _converters; }
        }

        ISchemeOperatorsBuilder IExpressionSchemeBuilder<TClass>.Operators
        {
            get { return _operators; }
        }

        ISchemeRoutesBuilder<TClass> IExpressionSchemeBuilder<TClass>.Routes
        {
            get { return _routes; }
        }

        public Func<TClass, bool> Build(IExpressionProvider provider)
        {
            return ExpressionBuilder.Build<TClass>(this, provider);
        }

        #endregion

        #region IExpressionSchemeProvider Members

        ISchemeOperatorProvider IExpressionSchemeProvider.Operators
        {
            get { return _operators; }
        }

        ISchemeRoutesProvider IExpressionSchemeProvider.Routes
        {
            get { return _routes; }
        }

        ISchemeConverterProvider IExpressionSchemeProvider.Converters
        {
            get { return _converters; }
        }

        #endregion
    }
}