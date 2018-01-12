using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Expression.Json
{
    public class ParseBranch
    {
        private ParseEnvironment _environment;

        #region Constructors

        public ParseBranch(ParseContext context, ParseEnvironment environment, string branchName, string assertName)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Context = context;
            BranchName = branchName;
            AssertName = assertName;
            Asserts = new List<AssertResult>();
            _environment = environment.Clone();
        }

        #endregion

        #region Properties

        public string AssertName { get; }

        public List<AssertResult> Asserts { get; }

        public string BranchName { get; }

        public ParseContext Context { get; }

        public Exception Error
        {
            get
            {
                if (!Asserts.Any()) return new ParserException("There is no assets in sequence", BranchName);
                return Asserts.LastOrDefault()?.Error;
            }
        }

        #endregion

        #region Override members

        public override string ToString()
        {
            var error = Error;
            if (error == null) return "Success";
            return error.Message;
        }

        #endregion

        #region Members

        public ParseBranch Assert(Func<JsonToken, ParseEnvironment, bool> factory)
        {
            return Assert(null, factory);
        }

        public ParseBranch Assert(string label, Func<JsonToken, ParseEnvironment, bool> factory)
        {
            if (Asserts.Any(a => a.Error != null)) return this;
            var currentOffset = Asserts.Aggregate(0, (agg, a) => agg + a.Length);

            var aheadToken = Context.GetToken(currentOffset);

            var assert = new AssertResultToken
            {
                Label = label,
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
                    if (!factory(aheadToken, _environment))
                    {
                        assert.Error = new ParserException($"Unexpected token {aheadToken}.", BranchName);
                    }
                }
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, BranchName, e);
            }

            return this;
        }

        public ParseBranch Assert(Action<ParseContext> factory)
        {
            return Assert(null, factory);
        }

        public ParseBranch Assert(string label, Action<ParseContext> factory)
        {
            if (Asserts.Any(a => a.Error != null)) return this;
            var currentOffset = Asserts.Aggregate(0, (agg, a) => agg + a.Length);
            var assert = new AssertResultBranch
            {
                Label = label,
                Index = Asserts.Count,
                BranchName = BranchName
            };

            Asserts.Add(assert);

            try
            {
                var localContext = Context.Branch(currentOffset, _environment);
                factory(localContext);

                ParseBranch branch;
                if (localContext.SuccessBranch != null)
                {
                    branch = localContext.SuccessBranch;
                    if (!ReferenceEquals(_environment, branch._environment))
                    {
                        _environment = branch._environment;
                    }
                }
                else branch = localContext.FailedBranch;
                if (branch != null) assert.Branch = branch;
                else assert.Error = new ParserException("There is no sequence checks", BranchName);
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, BranchName, e);
            }

            return this;
        }

        public ParseBranch Assert(Action<ParseEnvironment> action = null)
        {
            return Assert(null, action);
        }

        public ParseBranch Assert(string label, Action<ParseEnvironment> action = null)
        {
            if (Asserts.Any(a => a.Error != null)) return this;

            var assert = new AssertResultEmpty
            {
                Label = label,
                Index = Asserts.Count
            };

            Asserts.Add(assert);

            try
            {
                action?.Invoke(_environment);
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, BranchName, e);
            }
            return this;
        }

        public int GetTokensLength()
        {
            return Asserts.Aggregate(0, (agg, a) => agg + a.Length);
        }

        #endregion
    }
}