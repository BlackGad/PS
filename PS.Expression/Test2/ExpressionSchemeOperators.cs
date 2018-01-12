using System;
using System.Collections.Generic;
using System.Linq;
using PS.Expression.Test2.Fluent;

namespace PS.Expression.Test2
{
    public class ExpressionSchemeOperators
    {
        private readonly List<ExpressionOperator> _operators;

        #region Constructors

        public ExpressionSchemeOperators()
        {
            _operators = new List<ExpressionOperator>();
        }

        #endregion

        #region Members

        public ExpressionOperatorBuilder Construct<TSource, TResult>(string token)
        {
            return new ExpressionOperatorBuilder(this, typeof(TSource), typeof(TResult), token);
        }

        public ExpressionOperatorBuilder Construct<TSource>(string token)
        {
            return new ExpressionOperatorBuilder(this, typeof(TSource), typeof(bool), token);
        }

        public IEnumerable<ExpressionOperator> GetOperatorsForType(Type type)
        {
            return _operators.Where(o => type == o.AppliedTo);
        }

        public ExpressionSchemeOperators Register(ExpressionOperator op)
        {
            _operators.Add(op);
            return this;
        }

        #endregion
    }
}