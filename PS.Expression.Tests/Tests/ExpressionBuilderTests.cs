using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using PS.Expression.Test2;
using PS.Expression.Test2.Fluent;
using PS.Expression.Tests.TestReferences.ExpressionBuilderTests;

namespace PS.Expression.Tests.Tests
{
    [TestFixture]
    public class ExpressionBuilderTests
    {
        [Test]
        public void Test()
        {
            var scheme = new ExpressionScheme<License>();

            scheme.Operators
                  .Construct<Guid>("equals").Register( /*factory*/)
                  .Construct<string>("equals").Register( /*factory*/)
                  .Construct<string>("startWith").Register( /*factory*/)
                  .Construct<string>("endWith").Register( /*factory*/)
                  .Construct<string>("contains").Register( /*factory*/)
                  .Construct<IEnumerable>("any").Register( /*factory*/)
                  .Construct<string>("isUpper").Key("custom").Register( /*factory*/);

            scheme.Map
                  .Path(src => src.Id)
                  .Path(src => src.Template.Id)
                  .Path(src => src.Template.Name,
                        opt => opt.Operators
                                  .Reset()
                                  .Include("custom"))
                  .Path(src => src.Template.Description);

            scheme.Map.Subset(src => src.Claims)
                  .Path(src => src.Id)
                  .Path(src => src.Type)
                  .Path(src => src.Name);

            var builder = new ExpressionBuilder<License>(scheme);
            var licenses = ModelBuilder.CreateModel();
            var queryLicenses = licenses.Where(builder.Build()).ToList();
        }
    }
}