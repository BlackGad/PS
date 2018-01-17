using System;
using System.Collections.Generic;
using System.Linq;
using PS.Query.Configuration;

namespace PS.Query
{
    internal class SchemeOperators : ISchemeOperatorsBuilder,
                                     ISchemeOperatorProvider
    {
        private readonly List<Operator> _operators;

        #region Constructors

        public SchemeOperators()
        {
            _operators = new List<Operator>();
        }

        #endregion

        #region ISchemeOperatorProvider Members

        IEnumerable<ComplexOperator> ISchemeOperatorProvider.GetComplexOperators()
        {
            return _operators.OfType<ComplexOperator>();
        }

        IEnumerable<SimpleOperator> ISchemeOperatorProvider.GetOperatorsForType(Type type)
        {
            return _operators.OfType<SimpleOperator>().Where(o => type == o.SourceType);
        }

        #endregion

        #region ISchemeOperatorsBuilder Members

        public ComplexOperatorBuilder<TResult> Complex<TResult>(string token)
        {
            return new ComplexOperatorBuilder<TResult>(this, token);
        }

        public SimpleOperatorBuilder<TSource> Simple<TSource>(string token)
        {
            return new SimpleOperatorBuilder<TSource>(this, token);
        }

        #endregion

        #region Members

        public ISchemeOperatorsBuilder Register(Operator op)
        {
            _operators.Add(op);
            return this;
        }

        #endregion
    }
}