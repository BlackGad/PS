using PS.Expression.Logic;
using PS.Navigation;

namespace PS.Expression
{
    public class SimpleRoute
    {
        #region Properties

        public Route Route { get; set; }

        public RouteOperation RouteOperation { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            return $"{Route} {RouteOperation}";
        }

        #endregion
    }

    public class ComplexRoute : SimpleRoute
    {
        #region Properties

        public string ComplexOperator { get; set; }

        public LogicalExpression LogicalExpression { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            var message = $"{Route} {ComplexOperator} ({LogicalExpression})";
            if (RouteOperation != null) message += $" {RouteOperation}";
            return message;
        }

        #endregion
    }

    public class RouteOperation
    {
        #region Properties

        public string Operator { get; set; }

        public string Value { get; set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            return $"{Operator} {Value}";
        }

        #endregion
    }
}