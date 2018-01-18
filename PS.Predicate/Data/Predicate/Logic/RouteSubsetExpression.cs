using System.Collections.Generic;

namespace PS.Data.Predicate.Logic
{
    public class RouteSubsetExpression : RouteExpression
    {
        #region Properties

        public string Query { get; set; }
        public IExpression Subset { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var parts = new List<string>();
            parts.Add($"{Route}");
            if (!string.IsNullOrEmpty(Query)) parts.Add($"{Query}");
            if (Subset != null) parts.Add($"{{{Subset}}})");
            if (Operator != null) parts.Add($"{Operator}");
            return string.Join(" ", parts);
        }

        #endregion
    }
}