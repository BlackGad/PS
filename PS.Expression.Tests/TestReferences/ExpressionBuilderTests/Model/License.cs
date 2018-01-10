using System;
using System.Collections.Generic;

namespace PS.Expression.Tests.TestReferences.ExpressionBuilderTests.Model
{
    public class License
    {
        #region Properties

        public List<Claim> Claims { get; set; }
        public Guid Id { get; set; }
        public Template Template { get; set; }

        #endregion
    }
}