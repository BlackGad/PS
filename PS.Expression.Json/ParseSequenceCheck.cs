using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Expression.Json
{
    public class ParseSequenceCheck
    {
        private readonly List<AssertResult> _asserts;
        private readonly ParseContext _context;
        private readonly string _sourceRuleName;
        private int _assertsLength;

        #region Constructors

        public ParseSequenceCheck(ParseContext context, string sourceRuleName)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
            _sourceRuleName = sourceRuleName;
            _asserts = new List<AssertResult>();
            _assertsLength = 0;
        }

        #endregion

        #region Properties

        public Exception Error
        {
            get
            {
                if (!_asserts.Any()) return new ParserException("There is no assets in sequence", _sourceRuleName);
                return _asserts.LastOrDefault()?.Error;
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

        public virtual ParseSequenceCheck Assert(Func<JsonToken, Dictionary<object, object>, bool> factory)
        {
            if (_asserts.Any(a => a.Error != null)) return this;
            var currentOffset = _asserts.Aggregate(0, (agg, a) => agg + a.Length);

            var aheadToken = _context.GetToken(currentOffset);

            var assert = new TokenAssertResult
            {
                Token = aheadToken
            };

            _asserts.Add(assert);

            try
            {
                if (aheadToken == null) assert.Error = new ParserException("Unexpected end of sequence.", _sourceRuleName);
                else
                {
                    if (!factory(aheadToken, _context.Environment))
                    {
                        assert.Error = new ParserException($"Unexpected token {aheadToken}.", _sourceRuleName);
                    }
                }
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, _sourceRuleName, e);
            }

            _assertsLength += assert.Length;

            return this;
        }

        public virtual ParseSequenceCheck Assert(Action<ParseContext> factory)
        {
            if (_asserts.Any(a => a.Error != null)) return this;
            var currentOffset = _asserts.Aggregate(0, (agg, a) => agg + a.Length);
            var assert = new BranchAssertResult();

            _asserts.Add(assert);

            try
            {
                var localContext = _context.Branch(currentOffset);
                factory(localContext);
                var branch = localContext.SuccessBranch ?? localContext.FailedBranch;
                if (branch != null) assert.Branch = branch;
                else
                {
                    assert.Error = new ParserException("There is no sequence checks", _sourceRuleName);
                }
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, _sourceRuleName, e);
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

    public abstract class AssertResult
    {
        #region Properties

        public virtual Exception Error { get; set; }

        public abstract int Length { get; }

        #endregion
    }

    public class TokenAssertResult : AssertResult
    {
        #region Properties

        public override int Length
        {
            get { return 1; }
        }

        public JsonToken Token { get; set; }

        #endregion
    }

    public class BranchAssertResult : AssertResult
    {
        #region Properties

        public ParseSequenceCheck Branch { get; set; }

        public override Exception Error
        {
            get { return base.Error ?? Branch?.Error; }
            set { base.Error = value; }
        }

        public override int Length
        {
            get { return Branch?.GetAssertsLength() ?? 0; }
        }

        #endregion
    }
}