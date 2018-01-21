namespace PS.Data.Predicate.New
{
    class Tokens : DescriptorStorage<Tokens, JsonToken>
    {
        #region Static members

        public static JsonToken And
        {
            get { return FromCache(() => new JsonToken(TokenType.And)); }
        }

        public static JsonToken ArrayEnd
        {
            get { return FromCache(() => new JsonToken(TokenType.ArrayEnd)); }
        }

        public static JsonToken ArrayStart
        {
            get { return FromCache(() => new JsonToken(TokenType.ArrayStart)); }
        }

        public static JsonToken EOS
        {
            get { return FromCache(() => new JsonToken(TokenType.EOS)); }
        }

        public static JsonToken Not
        {
            get { return FromCache(() => new JsonToken(TokenType.Not)); }
        }

        public static JsonToken Object
        {
            get { return FromCache(() => new JsonToken(TokenType.Object)); }
        }

        public static JsonToken Operator
        {
            get { return FromCache(() => new JsonToken(TokenType.Operator)); }
        }

        public static JsonToken Or
        {
            get { return FromCache(() => new JsonToken(TokenType.Or)); }
        }

        public static JsonToken Value
        {
            get { return FromCache(() => new JsonToken(TokenType.Value)); }
        }

        #endregion
    }
}