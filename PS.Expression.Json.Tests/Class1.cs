using System;
using System.IO;
using NUnit.Framework;

namespace PS.Expression.Json.Tests
{
    [TestFixture]
    public class Tests
    {
        class License
        {
            #region Properties

            public Claim[] Claims { get; set; }
            public Guid Id { get; set; }
            public Template Template { get; set; }

            #endregion
        }

        class Template
        {
            #region Properties

            public Guid Id { get; set; }
            public string Name { get; set; }

            #endregion
        }

        class Claim
        {
            #region Properties

            public string Type { get; set; }
            public string Value { get; set; }

            #endregion
        }

        [Test]
        public void Test()
        {
            //var factorizedGroup = node.Condition.Factorize(new FactorizeParams(ConvertFactory));
            var schema = new FluentObjectSchema<License>();
            schema.Token(obj => obj.Id).Operator(nameof(Equals));
            schema.Next(obj => obj.Template).Token(obj => obj.Id).Operator(nameof(Equals));
            schema.Next(obj => obj.Template).Token(obj => obj.Name).Operator(nameof(Equals));

            var json = File.ReadAllText(@"e:\Temp\11111.json");
            var s = JsonExpressionVisitor.Parse(json);
        }
    }
}