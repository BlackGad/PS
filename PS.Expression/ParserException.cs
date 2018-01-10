using System;

namespace PS.Expression
{
    public class ParserException : InvalidOperationException
    {
        #region Constructors

        public ParserException(string message) : base("Invalid expression syntax: " + message)
        {
        }

        #endregion
    }
}