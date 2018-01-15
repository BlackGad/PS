using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PS.Extensions;

namespace PS.Query.Json
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

        public ParseContext(IEnumerable<JsonToken> tokens) : this(new ParseEnvironment())
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));
            _tokens = new List<JsonToken>(tokens);
        }

        private ParseContext(ParseEnvironment env)
        {
            _sequenceChecks = new List<ParseBranch>();
            Environment = env;
        }

        private ParseContext(List<JsonToken> tokens, ParseEnvironment env, int offset) : this(env)
        {
            _tokens = tokens;
            _offset = offset;
        }

        #endregion

        #region Properties

        public ParseEnvironment Environment { get; }

        public ParseBranch FailedBranch
        {
            get
            {
                if (!_sequenceChecks.Any() || SuccessBranch != null) return null;
                return _sequenceChecks.OrderByDescending(check => check.GetTokensLength()).First();
            }
        }

        public ParseBranch SuccessBranch
        {
            get
            {
                var successBranches = _sequenceChecks.Where(c => c.Error == null).ToList();
                if (!successBranches.Any()) return null;
                return successBranches.OrderByDescending(check => check.GetTokensLength()).First();
            }
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

        public ParseContext Branch(int offset, ParseEnvironment environment)
        {
            return new ParseContext(_tokens, environment, _offset + offset);
        }

        public JsonToken GetToken(int position = 0)
        {
            if (_offset + position >= 0 && _offset + position < _tokens.Count) return _tokens[_offset + position];
            return default(JsonToken);
        }

        public ParseBranch Sequence(string sequenceName, [CallerMemberName] string branchName = null)
        {
            var result = new ParseBranch(this, Environment, branchName, sequenceName);
            _sequenceChecks.Add(result);
            return result;
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture);
        }

        private string GenerateDGraph(ParseBranch branch, Guid id)
        {
            if (branch == null) throw new InvalidOperationException();
            var builder = new StringBuilder();
            builder.AppendLine($"{GenerateDGraphVertexId(id)} [color=chartreuse1] [label=\"{branch.BranchName}\"]");

            foreach (var assert in branch.Asserts.Enumerate().Reverse())
            {
                var branchAssertResult = assert as AssertResultBranch;
                if (branchAssertResult != null)
                {
                    var label = branchAssertResult.Label ?? branchAssertResult.Branch.BranchName;
                    builder.AppendLine($"{GenerateDGraphVertexId(assert.Id)} [color=orange] " +
                                       $"[label=\"{label}\"]");
                    builder.AppendLine($"{GenerateDGraphVertexId(id)} -> {GenerateDGraphVertexId(assert.Id)}");
                    builder.Append(GenerateDGraph(branchAssertResult.Branch, assert.Id));
                }

                var tokenAssertResult = assert as AssertResultToken;
                if (tokenAssertResult != null)
                {
                    var color = "darkslategray1";
                    var label = tokenAssertResult.Label ?? tokenAssertResult.Token.ToString().Replace("\"", string.Empty);
                    var tooltip = string.Empty;
                    if (assert.Error != null)
                    {
                        color = "red";
                        tooltip = assert.Error.Message;
                    }
                    builder.AppendLine($"{GenerateDGraphVertexId(assert.Id)} [color={color}]" +
                                       $"[label=\"{label}\"]" +
                                       $"[tooltip=\"{tooltip}\"]");
                    builder.AppendLine($"{GenerateDGraphVertexId(id)} -> {GenerateDGraphVertexId(assert.Id)}");
                }

                var tokenAssertEmpty = assert as AssertResultEmpty;
                if (tokenAssertEmpty != null)
                {
                    var label = tokenAssertEmpty.Label ?? "Empty";
                    builder.AppendLine($"{GenerateDGraphVertexId(assert.Id)} [label=\"{label}\"]");
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