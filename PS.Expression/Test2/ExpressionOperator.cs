using System;

namespace PS.Expression.Test2
{
    public class ExpressionOperator
    {
        #region Properties

        public Type AppliedTo { get; set; }
        public Func<object> Factory { get; set; }

        public string Token { get; set; }
        public string Key { get; set; }

        #endregion
    }
}