using System;
using PS.Expression.Test2;
using PS.Expression.Tests.TestReferences.ExpressionBuilderTests.Model;

namespace PS.Expression.Tests.TestReferences.ExpressionBuilderTests
{
    class TestVisitor : ExpressionVisitor<License>
    {
        #region Override members

        public override Func<License, bool> Visit(ExpressionScheme<License> scheme)
        {
            return null;
        }

        #endregion
    }
}