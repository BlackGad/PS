namespace PS.Expression.Json
{
    public enum TokenType
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