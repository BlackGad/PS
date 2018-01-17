using PS.Navigation;

namespace PS.Query
{
    internal interface ISchemeRoutesProvider
    {
        #region Members

        SchemeRouteComplex GetComplexRoute(Route route);
        SchemeRoute GetRoute(Route route);

        #endregion
    }
}