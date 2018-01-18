using System;
using System.Collections;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PS.Data.Logic;

namespace PS.Data.Predicate.Logic
{
    public class LogicalExpression : ILogicalExpression,
                                     IExpression,
                                     IXmlSerializable
    {
        #region Constructors

        public LogicalExpression(LogicalOperator op, IExpression[] expressions)
        {
            Expressions = expressions;
            Operator = op;
        }

        private LogicalExpression()
        {
        }

        #endregion

        #region Properties

        public IExpression[] Expressions { get; }

        #endregion

        #region Override members

        public override string ToString()
        {
            return string.Join($" {Operator.ToString().ToUpperInvariant()} ", Expressions.Select(o => "(" + o.ToString() + ")"));
        }

        #endregion

        #region ILogicalExpression Members

        public LogicalOperator Operator { get; }

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

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            throw new NotSupportedException();
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
        }

        #endregion
    }
}