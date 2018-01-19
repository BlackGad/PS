using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PS.Data.Predicate.Serialization;
using PS.Navigation;

namespace PS.Data.Predicate.Logic
{
    [XmlRoot("Expression")]
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

        #region IExpression Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new System.NotSupportedException();
        }

        public virtual void ReadXml(XmlReader reader)
        {
            Operator = ExpressionSerialization.ReadOperatorExpression(reader);
            Route = ExpressionSerialization.ReadExpressionRoute(reader);
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            ExpressionSerialization.WriteExpressionRoute(writer, Route);
            if (Operator != null) ExpressionSerialization.WriteOperatorExpression(writer, Operator);
        }

        #endregion
    }
}