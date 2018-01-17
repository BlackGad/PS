namespace PS.Data.Logic
{
    public class InvertedLogicalExpression : IInvertedLogicalExpression
    {
        #region Constructors

        public InvertedLogicalExpression(object expression)
        {
            Expression = expression;
        }

        #endregion

        #region Properties

        public object Expression { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"NOT {Expression}";
        }

        #endregion
    }
}