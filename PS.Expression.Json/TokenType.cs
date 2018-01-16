namespace PS.Query.Json
{
    enum TokenType
    {
        Undefined,
        ArrayStart,
        ArrayEnd,
        Object,
        And,
        Or,
        Not,
        Operator,
        Value,
        EOS,
        Any,
    }
}