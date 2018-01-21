using PS.Data.Parser;

namespace PS.Data.Predicate.New
{
    class Sequence<TToken> where TToken : IToken
    {
        #region Members

        public Sequence<TToken> Rule(Rule<TToken> rule)
        {
            return this;
        }

        public Sequence<TToken> Token(TToken token)
        {
            return this;
        }

        #endregion
    }
}