using PS.Expression.Test2.Fluent;

namespace PS.Expression.Test2
{
    public class ExpressionSchemeOperators
    {
        #region Members

        public ExpressionOperatorBuilder Construct<TSource, TResult>(string token)
        {
            return new ExpressionOperatorBuilder(this, typeof(TSource), typeof(TResult), token);
        }

        public ExpressionOperatorBuilder Construct<TSource>(string token)
        {
            return new ExpressionOperatorBuilder(this, typeof(TSource), typeof(bool), token);
        }

        public ExpressionSchemeOperators Register(ExpressionOperator op)
        {
            return this;
        }

        #endregion
    }
}