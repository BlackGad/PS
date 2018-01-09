using System;
using System.Security.Claims;
using PS.Expression.Test2;
using PS.Expression.Tests.TestReferences.ExpressionBuilderTests.Model;

namespace PS.Expression.Tests.TestReferences.ExpressionBuilderTests
{
    class TestVisitor : ExpressionVisitor<License>
    {
        #region Override members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheme"></param>
        /// <example>
        /// id equal FA36C38F-C1FF-4740-8F1E-5D0D5D76E508
        /// </example>
        /// <returns></returns>
        public override Func<License, bool> Visit(ExpressionScheme<License> scheme)
        {
            //var context = scheme.CreateReaderContext();
            //context.
            


            return null;
        }

        #endregion
    }
}