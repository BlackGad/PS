using System.Collections.Generic;

namespace PS.Query.Data.Predicate.Logic
{
    public class RouteSubsetExpression : RouteExpression
    {
        #region Properties

        public string SubsetOperator { get; set; }
        public LogicalExpression Sub { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var parts = new List<string>();
            parts.Add($"{Route}");
            if (!string.IsNullOrEmpty(SubsetOperator)) parts.Add($"{SubsetOperator}");
            if (Sub != null) parts.Add($"SUB({Sub.Expressions?.Length ?? 0})");
            if (Operator != null) parts.Add($"{Operator}");
            return string.Join(" ", parts);
        }

        #endregion
    }
}