using PS.Query.Configuration;

namespace PS.Query
{
    public interface ISchemeOperatorsBuilder
    {
        #region Members

        ComplexOperatorBuilder<TResult> Complex<TResult>(string token);
        ISchemeOperatorsBuilder Register(Operator op);
        SimpleOperatorBuilder<TSource> Simple<TSource>(string token);

        #endregion
    }
}