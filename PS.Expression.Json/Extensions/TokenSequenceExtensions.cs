using System.Runtime.CompilerServices;

namespace PS.Expression.Json.Extensions
{
    public static class TokenSequenceExtensions
    {
        #region Static members

        public static void CheckAheadToken(this TokenSequence<JsonToken> sequence, TokenType type, [CallerMemberName] string ruleName = null)
        {
            if (!sequence.IsTokenAvailable())
            {
                throw new ParserException($"There is no appropriate tokens. Rule '{ruleName}' rule.");
            }
            if (type == TokenType.Any) return;

            if (sequence.Lookahead().Type != type)
            {
                throw new ParserException($"There is unexpected {sequence.Lookahead()} token in {ruleName} rule. Expected: {type}");
            }
        }

        public static void ThrowUnexpectedToken(this JsonToken token, [CallerMemberName] string ruleName = null)
        {
            throw new ParserException($"Unexpected {token} token detected in {ruleName} rule.");
        }

        #endregion
    }
}