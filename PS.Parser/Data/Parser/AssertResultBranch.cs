using System;

namespace PS.Data.Parser
{
    internal class AssertResultBranch<TToken> : AssertResult
    {
        #region Properties

        public ParseBranch<TToken> Branch { get; set; }

        public override Exception Error
        {
            get { return base.Error ?? Branch?.Error; }
            set { base.Error = value; }
        }

        public override int Length => Branch?.GetTokensLength() ?? 0;

        #endregion
    }
}