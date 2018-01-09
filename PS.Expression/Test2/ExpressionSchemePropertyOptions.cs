using PS.Expression.Test2.Fluent;

namespace PS.Expression.Test2
{
    public class ExpressionSchemePropertyOptions
    {
        #region Constructors

        public ExpressionSchemePropertyOptions()
        {
            Operators = new ExpressionOperatorReferencesBuilder();
        }

        #endregion

        #region Properties

        public ExpressionOperatorReferencesBuilder Operators { get; }

        #endregion
    }
}