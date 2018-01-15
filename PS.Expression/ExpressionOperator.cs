using System;

namespace PS.Query
{
    public class ExpressionOperator
    {
        #region Properties

        public Type AppliedTo { get; set; }
        public Func<System.Linq.Expressions.Expression, object, System.Linq.Expressions.Expression> ExpressionFactory { get; set; }
        public string Key { get; set; }
        public Type ProducedResult { get; set; }

        public string Token { get; set; }

        #endregion
    }
}