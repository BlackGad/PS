using System.Collections.Generic;

namespace PS.Query.Model
{
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