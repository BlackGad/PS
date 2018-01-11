using System;

namespace PS.Expression.Json
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

        public virtual Exception Error { get; set; }
        public Guid Id { get; }

        public int Index { get; set; }

        public abstract int Length { get; }

        public string RuleName { get; set; }

        #endregion
    }
}