namespace PS.Parser
{
    internal class AssertResultToken<TToken> : AssertResult
    {
        #region Properties

        public override int Length => 1;

        public TToken Token { get; set; }

        #endregion
    }
}