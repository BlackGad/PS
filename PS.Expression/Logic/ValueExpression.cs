using System;

namespace PS.Expression.Logic
{
    public class ValueExpression : IValueExpression
    {
        #region Constructors

        public ValueExpression(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        #endregion

        #region Properties

        public string Value { get; }

        #endregion

        #region Override members

        public override string ToString()
        {
            return $"{Value}";
        }

        #endregion
    }
}