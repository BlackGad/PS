using System.Collections.Generic;
using PS.Navigation;

namespace PS.Query.Model
{
    public class RouteExpression
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

    public class ComplexRouteExpression : RouteExpression
    {
        #region Properties

        public string ComplexOperator { get; set; }
        public LogicalExpression Sub { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var parts = new List<string>();
            parts.Add($"{Route}");
            if (!string.IsNullOrEmpty(ComplexOperator)) parts.Add($"{ComplexOperator}");
            if (Sub != null) parts.Add($"SUB({Sub.Expressions?.Length ?? 0})");
            if (Operator != null) parts.Add($"{Operator}");
            return string.Join(" ", parts);
        }

        #endregion
    }

    public class OperatorExpression
    {
        #region Properties

        public bool Inverted { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var parts = new List<string>();

            if (Inverted) parts.Add("NOT");
            if (!string.IsNullOrEmpty(Operator)) parts.Add($"{Operator}");
            if (!string.IsNullOrEmpty(Value)) parts.Add($"{Value}");
            return string.Join(" ", parts);
        }

        #endregion
    }
}