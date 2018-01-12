using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Expression.Json
{
    public class ParseBranch
    {
        private readonly Dictionary<object, object> _environment;

        #region Constructors

        public ParseBranch(ParseContext context, Dictionary<object, object> environment, string ruleName, string assertName)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Context = context;
            RuleName = ruleName;
            AssertName = assertName;
            Asserts = new List<AssertResult>();
            _environment = new Dictionary<object, object>();
            foreach (var pair in environment)
            {
                _environment.Add(pair.Key, pair.Value);
            }
        }

        #endregion

        #region Properties

        public string AssertName { get; }

        public List<AssertResult> Asserts { get; }

        public ParseContext Context { get; }

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

        public ParseBranch Assert(Func<JsonToken, Dictionary<object, object>, bool> factory)
        {
            if (Asserts.Any(a => a.Error != null)) return this;
            var currentOffset = Asserts.Aggregate(0, (agg, a) => agg + a.Length);

            var aheadToken = Context.GetToken(currentOffset);

            var assert = new AssertResultToken
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
                    if (!factory(aheadToken, _environment))
                    {
                        assert.Error = new ParserException($"Unexpected token {aheadToken}.", RuleName);
                    }
                }
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, RuleName, e);
            }

            return this;
        }

        public ParseBranch Assert(Action<ParseContext> factory)
        {
            if (Asserts.Any(a => a.Error != null)) return this;
            var currentOffset = Asserts.Aggregate(0, (agg, a) => agg + a.Length);
            var assert = new AssertResultBranch
            {
                Index = Asserts.Count,
                RuleName = RuleName
            };

            Asserts.Add(assert);

            try
            {
                var localContext = Context.Branch(currentOffset, _environment);
                factory(localContext);

                var branch = localContext.SuccessBranch ?? localContext.FailedBranch;
                if (branch != null) assert.Branch = branch;
                else assert.Error = new ParserException("There is no sequence checks", RuleName);
            }
            catch (Exception e)
            {
                assert.Error = new ParserException(null, RuleName, e);
            }

            return this;
        }

        public void AssertEmpty()
        {
            if (Asserts.Any(a => a.Error != null)) return;

            Asserts.Add(new AssertResultEmpty());
        }

        public int GetAssertsLength()
        {
            return Asserts.Aggregate(0, (agg, a) => agg + a.Length);
        }

        #endregion
    }
}