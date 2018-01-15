namespace PS.Query.Json
{
    public class AssertResultToken : AssertResult
    {
        #region Properties

        public override int Length => 1;

        public JsonToken Token { get; set; }

        #endregion
    }
}