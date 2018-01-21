namespace PS.Data.Predicate.New
{
    class Rules : DescriptorStorage<Rules, Rule<JsonToken>>
    {
        #region Static members

        public static Rule<JsonToken> EXPRESSION
        {
            get
            {
                return FromCache(() => new Rule<JsonToken>
                {
                    s => s.Token(Tokens.Object)
                          .Rule(OPERATION),
                    s => s.Token(Tokens.Object)
                          .Token(Tokens.Operator)
                          .Rule(EXPRESSION_CONDITION)
                          .Rule(OPERATION),
                    s => s.Rule(EXPRESSION_CONDITION)
                });
            }
        }

        public static Rule<JsonToken> EXPRESSION_CONDITION
        {
            get
            {
                return FromCache(() => new Rule<JsonToken>
                {
                    s => s.Token(Tokens.And)
                          .Rule(EXPRESSION_CONDITION_BODY),
                    s => s.Token(Tokens.Or)
                          .Rule(EXPRESSION_CONDITION_BODY),
                    s => s.Rule(EXPRESSION_CONDITION_BODY)
                });
            }
        }

        public static Rule<JsonToken> EXPRESSION_CONDITION_BODY
        {
            get
            {
                return FromCache(() => new Rule<JsonToken>
                {
                    s => s.Token(Tokens.ArrayStart)
                          .Rule(EXPRESSION)
                          .Token(Tokens.ArrayEnd)
                });
            }
        }

        public static Rule<JsonToken> OPERATION
        {
            get
            {
                return FromCache(() => new Rule<JsonToken>
                {
                    s => s.Token(Tokens.Not)
                          .Token(Tokens.Operator)
                          .Token(Tokens.Value)
                });
            }
        }

        #endregion
    }
}