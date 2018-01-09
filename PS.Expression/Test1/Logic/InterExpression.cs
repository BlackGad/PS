using System;

namespace PS.Expression.Test1.Logic
{
    public class InterExpression
    {
        private readonly string _command;
        private readonly object _subExpression;

        #region Constructors

        public InterExpression(string command, object subExpression)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (subExpression == null) throw new ArgumentNullException(nameof(subExpression));
            _command = command;
            _subExpression = subExpression;
        }

        #endregion

        #region Override members

        public override string ToString()
        {
            return $"{_command} : {_subExpression}";
        }

        #endregion
    }
}