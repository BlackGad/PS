using System;

namespace PS.Data.Parser
{
    public class ParseRuleBranch<TToken> : IParseBranch<TToken> where TToken : IToken
    {
        private readonly ParseBranch<TToken> _branch;
        private readonly ParseEnvironment _ruleEnvironment;
        private readonly bool _skip;

        #region Constructors

        internal ParseRuleBranch(ParseBranch<TToken> branch, ParseEnvironment ruleEnvironment = null, bool skip = true)
        {
            _branch = branch;
            _ruleEnvironment = ruleEnvironment;
            _skip = skip;
        }

        #endregion

        #region IParseBranch<TToken> Members

        public ParseRuleBranch<TToken> Rule(Action<ParseContext<TToken>> factory)
        {
            return _branch.Rule(factory);
        }

        public ParseTokenBranch<TToken> Token(string token)
        {
            return _branch.Token(token);
        }

        public void Commit(Action<ParseEnvironment> action)
        {
            _branch.Commit(action);
        }

        #endregion

        #region Members

        public ParseBranch<TToken> Action(Action<ParseEnvironment, ParseEnvironment> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!_skip) action(_branch.Environment, _ruleEnvironment);
            return _branch;
        }

        #endregion
    }
}