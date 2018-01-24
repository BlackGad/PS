using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Data.Parser
{
    public class ParseBranch<TToken> : IParseBranch<TToken> where TToken : IToken
    {
        #region Constructors

        internal ParseBranch(ParseContext<TToken> context, ParseEnvironment environment, string branchName, string assertName)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Context = context;
            BranchName = branchName;
            AssertName = assertName;
            Asserts = new List<AssertResult>();
            Environment = environment;
        }

        #endregion

        #region Properties

        public ParseEnvironment Environment { get; }

        public Exception Error
        {
            get
            {
                if (!Asserts.Any()) return new ParserException("There is no assets in sequence", BranchName);
                return Asserts.LastOrDefault()?.Error;
            }
        }

        internal string AssertName { get; }

        internal List<AssertResult> Asserts { get; }

        internal string BranchName { get; }

        internal ParseContext<TToken> Context { get; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var error = Error;
            if (error == null) return "Success";
            return error.Message;
        }

        #endregion

        #region IParseBranch<TToken> Members

        public ParseRuleBranch<TToken> Rule(Action<ParseContext<TToken>> factory)
        {
            if (Asserts.Any(a => a.Error != null)) return new ParseRuleBranch<TToken>(this);
            var currentOffset = Asserts.Aggregate(0, (agg, a) => agg + a.Length);
            var assert = new AssertResultBranch<TToken>
            {
                Index = Asserts.Count,
                BranchName = BranchName
            };

            Asserts.Add(assert);
            try
            {
                var localContext = Context.Branch(currentOffset);
                factory(localContext);

                var branch = localContext.SuccessBranch ?? localContext.FailedBranch;

                if (branch != null) assert.Branch = branch;
                else assert.Error = new ParserException("There is no sequence checks", BranchName);
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, BranchName, e);
            }

            return new ParseRuleBranch<TToken>(this, assert.Branch.Environment, assert.Error != null);
        }

        public ParseTokenBranch<TToken> Token(string token)
        {
            if (Asserts.Any(a => a.Error != null)) return new ParseTokenBranch<TToken>(this);
            var currentOffset = Asserts.Aggregate(0, (agg, a) => agg + a.Length);

            var aheadToken = Context.GetToken(currentOffset);

            var assert = new AssertResultToken<TToken>
            {
                Index = Asserts.Count,
                Token = aheadToken,
                BranchName = BranchName
            };

            Asserts.Add(assert);

            try
            {
                if (aheadToken == null) assert.Error = new ParserException("Unexpected end of sequence.", BranchName);
                else
                {
                    var expectedToken = Context.TokenTable[token];
                    if (expectedToken?.Equals(aheadToken) != true)
                    {
                        assert.Error = new ParserException($"Unexpected token '{aheadToken}'. " +
                                                           $"'{expectedToken?.ToString() ?? "unknown"}' token expected.",
                                                           BranchName);
                    }
                }
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, BranchName, e);
            }

            return new ParseTokenBranch<TToken>(this, aheadToken, assert.Error != null);
        }

        public void Commit(Action<ParseEnvironment> action)
        {
            if (Asserts.Any(a => a.Error != null)) return;

            var assert = new AssertResultEmpty
            {
                Index = Asserts.Count
            };

            Asserts.Add(assert);

            try
            {
                action?.Invoke(Environment);
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, BranchName, e);
            }
        }

        #endregion

        #region Members

        internal int GetTokensLength()
        {
            return Asserts.Aggregate(0, (agg, a) => agg + a.Length);
        }

        #endregion
    }

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