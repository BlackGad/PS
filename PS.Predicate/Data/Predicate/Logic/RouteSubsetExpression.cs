using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using PS.Data.Predicate.Serialization;

namespace PS.Data.Predicate.Logic
{
    [XmlRoot("Subset")]
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

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            Query = ExpressionSerialization.ReadSubsetExpressionQuery(reader);

            using (var subTree = reader.ReadSubtree())
            {
                //Skip root node
                subTree.MoveToContent();
                subTree.Read();

                if (subTree.IsStartElement())
                {
                    Subset = ExpressionSerialization.ReadNode(subTree);
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            ExpressionSerialization.WriteSubsetExpressionQuery(writer, Query);
            if (Subset != null) ExpressionSerialization.WriteNode(writer, Subset);
        }

        #endregion
    }
}