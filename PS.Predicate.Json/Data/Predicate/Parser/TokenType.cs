namespace PS.Data.Predicate.Parser
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