using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using PS.Expression.Test1;
using PS.Expression.Test2;
using PS.Expression.Tests.TestReferences.ExpressionBuilderTests.Model;

namespace PS.Expression.Json.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Parser()
        {
            var scheme = new ExpressionScheme<License>();

            scheme.Operators
                  .Construct<Guid>("equal").Register( /*factory*/)
                  .Construct<string>("equal").Register( /*factory*/)
                  .Construct<string>("startWith").Register( /*factory*/)
                  .Construct<string>("endWith").Register( /*factory*/)
                  .Construct<string>("contains").Register( /*factory*/)
                  .Construct<IEnumerable>("any").Register( /*factory*/)
                  .Construct<string>("isUpper").Key("custom").Register( /*factory*/);

            scheme.Map
                  .Route(src => src.Id)
                  .Route(src => src.Template.Id)
                  .Route(src => src.Template.Name,
                         opt => opt.Operators
                                   .Reset()
                                   .Include("custom"))
                  .Route(src => src.Template.Description);

            scheme.Map.SubRoute(src => src.Claims)
                  .Route(src => src.Id)
                  .Route(src => src.Type)
                  .Route(src => src.Name);

            var json = File.ReadAllText(@"D:\GitHub\PS\PS.Expression.Json.Tests\TextFile1.txt");
            var parser = new JsonParser<License>(scheme);
            var result = parser.Parse(json);
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