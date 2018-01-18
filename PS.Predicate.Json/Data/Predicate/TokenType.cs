namespace PS.Data.Predicate
{
    internal enum TokenType
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
        Any
    }
}