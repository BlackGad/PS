using System;

namespace PS.Query
{
    internal interface ISchemeConverterProvider
    {
        #region Members

        object Convert(string value, Type type);

        #endregion
    }
}