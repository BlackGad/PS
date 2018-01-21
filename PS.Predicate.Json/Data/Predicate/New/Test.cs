using System.Linq;

namespace PS.Data.Predicate.New
{
    public class Test
    {
        #region Constructors

        public Test()
        {
            var s = Rules.EXPRESSION;
            var tokens = s.EntryTokens;
            var s2 = Rules.All.ToList();
        }

        #endregion
    }
}