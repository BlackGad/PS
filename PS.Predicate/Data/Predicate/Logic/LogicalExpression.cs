using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PS.Data.Logic;
using PS.Data.Predicate.Serialization;
using PS.Extensions;

namespace PS.Data.Predicate.Logic
{
    [XmlRoot("Logical")]
    public class LogicalExpression : ILogicalExpression,
                                     IExpression
    {
        #region Constructors

        public LogicalExpression(LogicalOperator op, IEnumerable<IExpression> expressions) : this()
        {
            Expressions.AddRange(expressions.Enumerate());
            Operator = op;
        }

        public LogicalExpression(LogicalOperator op, params IExpression[] expressions) :
            this(op, expressions.Enumerate())
        {
        }

        public LogicalExpression()
        {
            Operator = LogicalOperator.And;
            Expressions = new List<IExpression>();
        }

        #endregion

        #region Properties

        public List<IExpression> Expressions { get; private set; }

        #endregion

        #region Override members

        public override string ToString()
        {
            return string.Join($" {Operator.ToString().ToUpperInvariant()} ", Expressions.Select(o => "(" + o.ToString() + ")"));
        }

        #endregion

        #region IExpression Members

        public XmlSchema GetSchema()
        {
            throw new NotSupportedException();
        }

        public virtual void ReadXml(XmlReader reader)
        {
            var @operator = ExpressionSerialization.ReadExpressionOperator(reader);
            if (@operator != null) Operator = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), @operator, true);

            var result = new List<IExpression>();

            using (var subTree = reader.ReadSubtree())
            {
                //Skip root node
                subTree.MoveToContent();
                subTree.Read();

                while (subTree.IsStartElement())
                {
                    result.Add(ExpressionSerialization.ReadNode(subTree));
                    subTree.Read();
                }
            }

            reader.Skip();
            Expressions = result.ToList();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            ExpressionSerialization.WriteExpressionOperator(writer, Operator.ToString());
            foreach (var expression in Expressions)
            {
                ExpressionSerialization.WriteNode(writer, expression);
            }
        }

        #endregion

        #region ILogicalExpression Members

        public LogicalOperator Operator { get; private set; }

        IEnumerable ILogicalExpression.Expressions
        {
            get { return Expressions; }
        }

        void ILogicalExpression.AddExpression(object expression)
        {
            throw new NotSupportedException();
        }

        void ILogicalExpression.RemoveExpression(object expression)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}