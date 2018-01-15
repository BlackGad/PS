using System;

namespace PS.Query.Json
{
    public abstract class AssertResult
    {
        #region Constructors

        protected AssertResult()
        {
            Id = Guid.NewGuid();
        }

        #endregion

        #region Properties

        public string BranchName { get; set; }

        public virtual Exception Error { get; set; }
        public Guid Id { get; }

        public int Index { get; set; }
        public string Label { get; set; }

        public abstract int Length { get; }

        #endregion
    }
}