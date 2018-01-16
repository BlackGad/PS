using System;

namespace PS.Parser
{
    public class ParserException : InvalidOperationException
    {
        #region Constructors

        public ParserException(string message, string branchName, Exception innerException = null)
            : base($"Invalid expression syntax in {branchName} context" +
                   (string.IsNullOrWhiteSpace(message) ? string.Empty : ": " + message),
                   innerException)
        {
        }

        #endregion
    }
}