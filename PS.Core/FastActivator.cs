using System;
using System.Linq.Expressions;

namespace PS
{
    /// <summary>
    /// </summary>
    public static class FastActivator
    {
        #region Static members

        public static T Create<T>() where T : new()
        {
            return FastActivatorImpl<T>.NewFunction();
        }

        #endregion

        #region Nested type: FastActivatorImpl

        private static class FastActivatorImpl<T> where T : new()
        {
            #region Constants

            private static readonly Expression<Func<T>> NewExpression = () => new T();
            public static readonly Func<T> NewFunction = NewExpression.Compile();

            #endregion
        }

        #endregion
    }
}