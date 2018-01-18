using System.Collections.Generic;
using PS.Navigation;

namespace PS.Data.Predicate.Logic
{
    public class RouteExpression : IExpression
    {
        #region Properties

        public OperatorExpression Operator { get; set; }
        public Route Route { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var parts = new List<string>();
            parts.Add($"{Route}");
            if (Operator != null) parts.Add($"{Operator}");
            return string.Join(" ", parts);
        }

        #endregion
    }
}