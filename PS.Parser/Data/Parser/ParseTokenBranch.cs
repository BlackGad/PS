using System;

namespace PS.Data.Parser
{
    public class ParseTokenBranch<TToken> : IParseBranch<TToken> where TToken : IToken
    {
        private readonly TToken _aheadToken;
        private readonly ParseBranch<TToken> _branch;
        private readonly bool _skip;

        #region Constructors

        internal ParseTokenBranch(ParseBranch<TToken> branch, TToken aheadToken = default(TToken), bool skip = true)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));
            _branch = branch;
            _aheadToken = aheadToken;
            _skip = skip;
        }

        #endregion

        #region IParseBranch<TToken> Members

        public void Commit(Action<ParseEnvironment> action)
        {
            _branch.Commit(action);
        }

        public ParseRuleBranch<TToken> Rule(Action<ParseContext<TToken>> factory)
        {
            return _branch.Rule(factory);
        }

        public ParseTokenBranch<TToken> Token(string token)
        {
            return _branch.Token(token);
        }

        #endregion

        #region Members

        public ParseBranch<TToken> Action(Action<ParseEnvironment, TToken> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!_skip) action(_branch.Environment, _aheadToken);
            return _branch;
        }

        #endregion
    }
}