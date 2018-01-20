using System.Collections.Generic;

namespace PS.Data.Predicate.Logic
{
    public class OperatorExpression
    {
        #region Properties

        public bool Inverted { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var parts = new List<string>();

            if (Inverted) parts.Add("NOT");
            if (!string.IsNullOrEmpty(Name)) parts.Add($"{Name}");
            if (!string.IsNullOrEmpty(Value)) parts.Add($"'{Value}'");
            return string.Join(" ", parts);
        }

        #endregion
    }
}