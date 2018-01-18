using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PS.Query.Data.Predicate;
using PS.Query.Data.Predicate.Extensions;
using PS.Query.Tests.TestReferences.ExpressionBuilderTests;
using PS.Query.Tests.TestReferences.ExpressionBuilderTests.Model;

namespace PS.Query.Json.Tests
{
    [TestFixture]
    public class Tests
    {
        #region Test Parser

        [Test]
        public void Parser()
        {
            var scheme = Scheme.Create<License>();

            scheme.Routes
                  .Route(src => src.Id)
                  .Route(src => src.Template.Id)
                  .Route(src => src.Template.Name)
                  .Route(src => src.Template.Description);

            scheme.Routes.Subset(src => src.Claims)
                  .Route(src => src.Id)
                  .Route(src => src.Type)
                  .Route(src => src.Name);

            var json = File.ReadAllText(@"D:\GitHub\PS\PS.Query.Json.Tests\TextFile1.txt");
            var jToken = (JToken)JsonConvert.DeserializeObject(json);
            var provider = new JsonPredicateModelProvider(jToken);
            var licenses = ModelBuilder.CreateModel();
            var queryLicenses = licenses.Where(scheme.Build(provider)).ToList();
        }

        #endregion
    }
}