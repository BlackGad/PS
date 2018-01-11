namespace PS.Expression.Json
{
    public class TokenAssertResult : AssertResult
    {
        #region Properties

        public override int Length => 1;

        public JsonToken Token { get; set; }

        #endregion
    }
}