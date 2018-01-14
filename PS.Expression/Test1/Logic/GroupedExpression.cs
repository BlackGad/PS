using System;
using PS.Expression.Logic;

namespace PS.Expression.Test1.Logic
{
    public class GroupedExpression : LogicalExpression
    {
        private readonly string _command;

        #region Constructors

        public GroupedExpression(string command) : base(LogicalOperator.And)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            _command = command;
        }

        #endregion

        #region Override members

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{_command}: {base.ToString()}";
        }

        #endregion
    }
}