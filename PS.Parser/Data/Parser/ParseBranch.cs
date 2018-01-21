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
}