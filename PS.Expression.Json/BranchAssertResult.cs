using System;

namespace PS.Expression.Json
{
    public class BranchAssertResult : AssertResult
    {
        #region Properties

        public ParseBranch Branch { get; set; }
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