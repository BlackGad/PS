namespace PS.Query
{
    internal interface IExpressionSchemeProvider
    {
        #region Properties

        ISchemeConverterProvider Converters { get; }

        ISchemeOperatorProvider Operators { get; }

        ISchemeRoutesProvider Routes { get; }

        #endregion
    }
}