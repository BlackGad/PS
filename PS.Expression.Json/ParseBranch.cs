using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Expression.Json
{
    public class ParseBranch
    {
        private readonly ParseContext _context;
        private int _assertsLength;

        #region Constructors

        public ParseBranch(ParseContext context, string ruleName, string assertName)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
            RuleName = ruleName;
            AssertName = assertName;

            Asserts = new List<AssertResult>();
            _assertsLength = 0;
        }

        #endregion

        #region Properties

        public string AssertName { get; }

        public List<AssertResult> Asserts { get; }

        public Exception Error
        {
            get
            {
                if (!Asserts.Any()) return new ParserException("There is no assets in sequence", RuleName);
                return Asserts.LastOrDefault()?.Error;
            }
        }

        public string RuleName { get; }

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

        public virtual ParseBranch Assert(Func<JsonToken, Dictionary<object, object>, bool> factory)
        {
            if (Asserts.Any(a => a.Error != null)) return this;
            var currentOffset = Asserts.Aggregate(0, (agg, a) => agg + a.Length);

            var aheadToken = _context.GetToken(currentOffset);

            var assert = new TokenAssertResult
            {
                Index = Asserts.Count,
                Token = aheadToken,
                RuleName = RuleName
            };

            Asserts.Add(assert);

            try
            {
                if (aheadToken == null) assert.Error = new ParserException("Unexpected end of sequence.", RuleName);
                else
                {
                    if (!factory(aheadToken, _context.Environment))
                    {
                        assert.Error = new ParserException($"Unexpected token {aheadToken}.", RuleName);
                    }
                }
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, RuleName, e);
            }

            _assertsLength += assert.Length;

            return this;
        }

        public virtual ParseBranch Assert(Action<ParseContext> factory)
        {
            if (Asserts.Any(a => a.Error != null)) return this;
            var currentOffset = Asserts.Aggregate(0, (agg, a) => agg + a.Length);
            var assert = new BranchAssertResult
            {
                Index = Asserts.Count,
                RuleName = RuleName
            };

            Asserts.Add(assert);

            try
            {
                var localContext = _context.Branch(currentOffset);
                factory(localContext);
                var branch = localContext.SuccessBranch ?? localContext.FailedBranch;
                if (branch != null) assert.Branch = branch;
                else
                {
                    assert.Error = new ParserException("There is no sequence checks", RuleName);
                }
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, RuleName, e);
            }

            _assertsLength += assert.Length;

            return this;
        }

        public int GetAssertsLength()
        {
            return _assertsLength;
        }

        #endregion
    }
}