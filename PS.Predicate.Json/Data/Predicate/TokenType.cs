namespace PS.Data.Predicate
{
    internal enum TokenType
    {
        ArrayStart,
        ArrayEnd,
        Object,
        And,
        Or,
        Not,
        Operator,
        Value,
        EOS
    }
}