using System;

namespace PS.Query
{
    public interface IExpressionSchemeBuilder<T>
    {
        #region Properties

        ISchemeConverterBuilder Converters { get; }

        ISchemeOperatorsBuilder Operators { get; }

        ISchemeRoutesBuilder<T> Routes { get; }

        #endregion

        #region Members

        Func<T, bool> Build(IExpressionProvider provider);

        #endregion
    }
}