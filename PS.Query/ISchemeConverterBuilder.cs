using System;

namespace PS.Query
{
    public interface ISchemeConverterBuilder
    {
        #region Members

        ISchemeConverterBuilder Register<T>(Func<string, T> converter);

        #endregion
    }
}