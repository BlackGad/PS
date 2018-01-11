using System;

namespace PS.Expression
{
    public class ParserException : InvalidOperationException
    {
        #region Constructors

        public ParserException(string message, string ruleName, Exception innerException = null)
            : base($"Invalid expression syntax in {ruleName} context" +
                   (string.IsNullOrWhiteSpace(message) ? string.Empty : ": " + message),
                   innerException)
        {
        }

        #endregion
    }
}