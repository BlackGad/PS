using PS.Data;

namespace PS.Navigation
{
    public sealed class Routes : DescriptorStorage<Routes, Route>
    {
        #region Static members

        public static Route Empty
        {
            get { return FromCache(() => Route.Create()); }
        }

        public static Route Wildcard
        {
            get { return FromCache(() => Route.Create(Route.Wildcard)); }
        }

        public static Route WildcardRecursive
        {
            get { return FromCache(() => Route.Create(Route.WildcardRecursive)); }
        }

        #endregion
    }
}