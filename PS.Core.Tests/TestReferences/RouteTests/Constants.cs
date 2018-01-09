using PS.Navigation;

namespace PS.Tests.TestReferences.RouteTests
{
    public static class Constants
    {
        #region Constants

        public static readonly object Object1;
        public static readonly object Object2;
        public static readonly object Object3;
        public static readonly object Object4;
        public static readonly object Object5;
        public static readonly object Object6;
        public static readonly object Object7;
        public static readonly object Object8;
        public static readonly object Object9;
        public static readonly object ObjectDot;

        public static readonly Route Route1;
        public static readonly Route Route12;
        public static readonly Route Route123;
        public static readonly Route Route1Dot2;
        public static readonly Route Route1R3;
        public static readonly Route Route1W3;
        public static readonly Route Route23;
        public static readonly Route Route3;
        public static readonly Route RouteDot;

        public static readonly string String1;
        public static readonly string String2;
        public static readonly string String3;
        public static readonly string StringDot;

        #endregion

        #region Constructors

        static Constants()
        {
            Object1 = 1;
            Object2 = 2;
            Object3 = 3;
            Object4 = 4;
            Object5 = 5;
            Object6 = 6;
            Object7 = 7;
            Object8 = 8;
            Object9 = 9;
            ObjectDot = ".";

            String1 = Object1.ToString();
            String2 = Object2.ToString();
            String3 = Object3.ToString();
            StringDot = ObjectDot.ToString();

            Route1 = Route.Create(Object1);
            Route12 = Route.Create(Object1, Object2);
            Route123 = Route.Create(Object1, Object2, Object3);
            Route1W3 = Route.Create(Object1, Routes.Wildcard, Object3);
            Route1R3 = Route.Create(Object1, Routes.WildcardRecursive, Object3);
            Route23 = Route.Create(Object2, Object3);
            Route3 = Route.Create(Object3);
            RouteDot = Route.Create(ObjectDot);
            Route1Dot2 = Route.Create(Object1, ObjectDot, Object2);
        }

        #endregion
    }
}