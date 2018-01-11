using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PS.Extensions;

namespace PS.Expression.Json
{
    public class ParseContext : IFormattable
    {
        #region Static members

        private static string GenerateDGraphVertexId(Guid id)
        {
            return "v" + id.ToString("N");
        }

        #endregion

        private readonly int _offset;

        private readonly List<ParseBranch> _sequenceChecks;
        private readonly List<JsonToken> _tokens;

        #region Constructors

        public ParseContext(IEnumerable<JsonToken> tokens) : this()
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));
            _tokens = new List<JsonToken>(tokens);
        }

        private ParseContext()
        {
            _sequenceChecks = new List<ParseBranch>();
            Environment = new Dictionary<object, object>();
        }

        private ParseContext(List<JsonToken> tokens, int offset) : this()
        {
            _tokens = tokens;
            _offset = offset;
        }

        #endregion

        #region Properties

        public Dictionary<object, object> Environment { get; }

        public ParseBranch FailedBranch
        {
            get
            {
                if (!_sequenceChecks.Any() || SuccessBranch != null) return null;
                return _sequenceChecks.OrderByDescending(check => check.GetAssertsLength()).First();
            }
        }

        public ParseBranch SuccessBranch
        {
            get { return _sequenceChecks.FirstOrDefault(c => c.Error == null); }
        }

        #endregion

        #region Override members

        public override string ToString()
        {
            return ToString("n");
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case "D":
                case "d":
                    var builder = new StringBuilder();
                    builder.AppendLine("digraph G {");
                    builder.AppendLine("node [style=filled]");
                    builder.AppendLine("node [shape=box]");
                    builder.Append(GenerateDGraph(SuccessBranch ?? FailedBranch, Guid.NewGuid()));
                    builder.AppendLine("}");
                    return builder.ToString();
                case "N":
                case "n":
                    return base.ToString();
            }
            throw new NotSupportedException();
        }

        #endregion

        #region Members

        public ParseContext Branch(int offset)
        {
            var branch = new ParseContext(_tokens, _offset + offset);
            foreach (var pair in Environment)
            {
                branch.Environment.Add(pair.Key, pair.Value);
            }

            return branch;
        }

        public ParseBranch CheckSequence(string assertName, [CallerMemberName] string ruleName = null)
        {
            var result = _sequenceChecks.Any(s => s.Error == null)
                ? new BlankParseBranch(this, ruleName, assertName)
                : new ParseBranch(this, ruleName, assertName);
            _sequenceChecks.Add(result);
            return result;
        }

        public JsonToken GetToken(int position = 0)
        {
            if (_offset + position >= 0 && _offset + position < _tokens.Count) return _tokens[_offset + position];
            return default(JsonToken);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture);
        }

        private string GenerateDGraph(ParseBranch branch, Guid id)
        {
            if (branch == null) throw new InvalidOperationException();
            var builder = new StringBuilder();
            builder.AppendLine($"{GenerateDGraphVertexId(id)} [color=chartreuse1] [label=\"{branch.RuleName}\"]");

            foreach (var assert in branch.Asserts.Enumerate().Reverse())
            {
                var branchAssertResult = assert as BranchAssertResult;
                if (branchAssertResult != null)
                {
                    builder.AppendLine($"{GenerateDGraphVertexId(assert.Id)} [color=orange] " +
                                       $"[label=\"{branchAssertResult.Branch.RuleName}\"]");
                    builder.AppendLine($"{GenerateDGraphVertexId(id)} -> {GenerateDGraphVertexId(assert.Id)}");
                    builder.Append(GenerateDGraph(branchAssertResult.Branch, assert.Id));
                }

                var tokenAssertResult = assert as TokenAssertResult;
                if (tokenAssertResult != null)
                {
                    var color = "darkslategray1";
                    var tooltip = string.Empty;
                    if (assert.Error != null)
                    {
                        color = "red";
                        tooltip = assert.Error.Message;
                    }
                    builder.AppendLine($"{GenerateDGraphVertexId(assert.Id)} [color={color}]" +
                                       $"[label=\"{tokenAssertResult.Token.ToString().Replace("\"", string.Empty)}\"]" +
                                       $"[tooltip=\"{tooltip}\"]");
                    builder.AppendLine($"{GenerateDGraphVertexId(id)} -> {GenerateDGraphVertexId(assert.Id)}");
                }
            }

            builder.AppendLine($"subgraph cluster_{GenerateDGraphVertexId(Guid.NewGuid())} {{");
            builder.AppendLine($"label = \"{branch.AssertName}\"");
            foreach (var assert in branch.Asserts)
            {
                builder.AppendLine($"{GenerateDGraphVertexId(assert.Id)}");
            }
            builder.AppendLine("}");

            return builder.ToString();
        }

        #endregion
    }
}