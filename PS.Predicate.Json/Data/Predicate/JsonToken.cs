using PS.Data.Parser;

namespace PS.Data.Predicate
{
    internal class JsonToken : IToken
    {
        #region Constructors

        public JsonToken(TokenType type, string value = null)
        {
            Value = value;
            Type = type;
        }

        #endregion

        #region Properties

        public TokenType Type { get; }
        public string Value { get; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var result = Type.ToString();
            if (Value != null) result += ": " + Value;
            return result;
        }

        #endregion

        #region IToken Members

        public bool Equals(IToken other)
        {
            var token = other as JsonToken;
            return token?.Type == Type;
        }

        #endregion
    }
}