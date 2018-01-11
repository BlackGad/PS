using System;
using PS.Navigation;

namespace PS.Expression.Test2
{
    public class ExpressionSchemeRoute
    {
        #region Properties

        public Route Route { get; set; }
        public Type Type { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            return $"{Type.Name}: {Route}";
        }

        #endregion
    }
}