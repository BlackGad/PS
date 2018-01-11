using System;
using System.Collections.Generic;

namespace PS.Expression.Json
{
    public class BlankParseBranch : ParseBranch
    {
        #region Constructors

        public BlankParseBranch(ParseContext context, string ruleName, string assertName) : base(context, ruleName, assertName)
        {
        }

        #endregion

        #region Override members

        public override ParseBranch Assert(Func<JsonToken, Dictionary<object, object>, bool> factory)
        {
            return this;
        }

        public override ParseBranch Assert(Action<ParseContext> factory)
        {
            return this;
        }

        #endregion
    }
}