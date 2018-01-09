namespace PS.Expression.Test2
{
    public class ExpressionScheme<TClass>
    {
        #region Constructors

        public ExpressionScheme()
        {
            Operators = new ExpressionSchemeOperators();
            Map = new ExpressionSchemeProperties<TClass>();
        }

        #endregion

        #region Properties

        public virtual ExpressionSchemeOperators Operators { get; }
        public virtual ExpressionSchemeProperties<TClass> Map { get; }

        #endregion
    }
}