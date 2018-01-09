using System;

namespace PS.Navigation
{
    public class RouteRecursiveSplit
    {
        #region Constructors

        public RouteRecursiveSplit(Route prefix, Route recursive, Route postfix)
        {
            if (recursive == null) throw new ArgumentNullException(nameof(recursive));
            if (postfix == null) throw new ArgumentNullException(nameof(postfix));
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));
            Recursive = recursive;
            Postfix = postfix;
            Prefix = prefix;
        }

        #endregion

        #region Properties

        public Route Postfix { get; private set; }

        public Route Recursive { get; private set; }
        public Route Prefix { get; private set; }
        public Route Root { get; private set; }
        public Route Tail { get; private set; }

        #endregion
    }
}