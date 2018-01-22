using System;

namespace PS.Data.Parser
{
    public interface IParseBranch<TToken> where TToken : IToken
    {
        #region Members

        ParseRuleBranch<TToken> Rule(Action<ParseContext<TToken>> factory);
        ParseTokenBranch<TToken> Token(string token);
        void Commit(Action<ParseEnvironment> action);

        #endregion
    }
}