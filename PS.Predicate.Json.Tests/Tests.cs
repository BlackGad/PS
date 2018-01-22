using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using PS.Data.Predicate;
using PS.Data.Predicate.Extensions;
using PS.Data.Predicate.Logic;
using PS.Predicate.Tests.TestReferences.ExpressionBuilderTests;
using PS.Predicate.Tests.TestReferences.ExpressionBuilderTests.Model;

namespace PS.Predicate.Json.Tests
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

            //Parser
            var json = File.ReadAllText(@"D:\GitHub\PS\PS.Predicate.Json.Tests\TextFile1.txt");

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new ExpressionJsonConverter()
                }
            };

            var expression = JsonConvert.DeserializeObject<IExpression>(json, jsonSerializerSettings);

            //XML
            var serializer = new XmlSerializer(expression.GetType());
            using (var textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, expression);
                var str = textWriter.ToString();
                using (TextReader reader = new StringReader(str))
                {
                    var result = serializer.Deserialize(reader);
                }
            }

            var serialized = JsonConvert.SerializeObject(expression, jsonSerializerSettings);
            var deserialized = JsonConvert.DeserializeObject<IExpression>(serialized, jsonSerializerSettings);

            var licenses = ModelBuilder.CreateModel();
            var queryLicenses = licenses.Where(scheme.Build(expression)).ToList();
        }

        #endregion
    }
}