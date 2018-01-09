using System;

namespace PS.Expression.Test1.Logic
{
    public class HeadExpression
    {
        private readonly object _subExpression;

        #region Constructors

        public HeadExpression(object subExpression)
        {
            if (subExpression == null) throw new ArgumentNullException(nameof(subExpression));
            _subExpression = subExpression;
        }

        #endregion

        #region Override members

        public override string ToString()
        {
            return $"{_subExpression}";
        }

        #endregion
    }
}