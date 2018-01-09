using System;
using System.Linq.Expressions;

namespace PS.Expression
{
    public class FluentObjectSchema<TObject>
    {
        #region Members

        public FluentObjectSchema<TProperty> Next<TProperty>(Expression<Func<TObject, TProperty>> ssss)
        {
            return new FluentObjectSchema<TProperty>();
        }

        public FluentTokenSchema Token<TProperty>(Expression<Func<TObject, TProperty>> ssss)
        {
            return new FluentTokenSchema();
        }

        #endregion
    }

    public class FluentTokenSchema
    {
        #region Members

        public FluentTokenSchema Operator(string name)
        {
            return this;
        }

        #endregion
    }
}