using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PS.Query.Tests.TestReferences.ExpressionBuilderTests;
using PS.Query.Tests.TestReferences.ExpressionBuilderTests.Model;

namespace PS.Query.Json.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Parser()
        {
            var scheme = new ExpressionScheme<License>();
            scheme.Converters
                  .Register(s => Guid.Parse(s))
                  .Register(s => s);

            scheme.Operators
                  .Construct<Guid>("equal").Register((src, value) => Expression.Equal(src, Expression.Constant(value)))
                //.Construct<string>("equal").Register( /*factory*/)
                //.Construct<string>("startWith").Register( /*factory*/)
                //.Construct<string>("endWith").Register( /*factory*/)
                //.Construct<string>("contains").Register( /*factory*/)
                //.Construct<IEnumerable>("any").Register( /*factory*/)
                //.Construct<string>("match").Key("custom").Register( /*factory*/)
                ;

            scheme.Map
                  .Route(src => src.Id)
                  .Route(src => src.Template.Id)
                  .Route(src => src.Template.Name,
                         opt => opt.Operators
                                   .Reset()
                                   .Include("custom"))
                  .Route(src => src.Template.Description);

            scheme.Map.Complex(src => src.Claims)
                  .Route(src => src.Id)
                  .Route(src => src.Type)
                  .Route(src => src.Name);

            var json = File.ReadAllText(@"D:\GitHub\PS\PS.Expression.Json.Tests\TextFile1.txt");
            var jToken = (JToken)JsonConvert.DeserializeObject(json);
            var provider = new JsonExpressionProvider(jToken);
            var licenses = ModelBuilder.CreateModel();
            var queryLicenses = licenses.Where(scheme.Build(provider)).ToList();
        }
    }
}