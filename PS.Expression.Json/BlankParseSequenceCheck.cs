using System;
using System.Collections.Generic;

namespace PS.Expression.Json
{
    public class BlankParseSequenceCheck : ParseSequenceCheck
    {
        #region Constructors

        public BlankParseSequenceCheck(ParseContext context, string sourceRuleName) : base(context, sourceRuleName)
        {
        }

        #endregion

        #region Override members

        public override ParseSequenceCheck Assert(Func<JsonToken, Dictionary<object, object>, bool> factory)
        {
            return this;
        }

        public override ParseSequenceCheck Assert(Action<ParseContext> factory)
        {
            return this;
        }

        #endregion
    }
}