using System;
using System.Collections.Generic;
using System.Linq;
using PS.Query.Fluent;

namespace PS.Query
{
    public class SchemeOperators
    {
        private readonly List<Operator> _operators;

        #region Constructors

        public SchemeOperators()
        {
            _operators = new List<Operator>();
        }

        #endregion

        #region Members

        public ComplexOperatorBuilder<TResult> Complex<TResult>(string token)
        {
            return new ComplexOperatorBuilder<TResult>(this, token);
        }

        public SimpleOperatorBuilder<TSource> Simple<TSource>(string token)
        {
            return new SimpleOperatorBuilder<TSource>(this, token);
        }

        internal IEnumerable<ComplexOperator> GetComplexOperators()
        {
            return _operators.OfType<ComplexOperator>();
        }

        internal IEnumerable<SimpleOperator> GetOperatorsForType(Type type)
        {
            return _operators.OfType<SimpleOperator>().Where(o => type == o.SourceType);
        }

        internal SchemeOperators Register(Operator op)
        {
            _operators.Add(op);
            return this;
        }

        #endregion
    }
}