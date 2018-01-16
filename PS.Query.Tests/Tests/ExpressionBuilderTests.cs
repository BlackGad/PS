using NUnit.Framework;

namespace PS.Query.Tests.Tests
{
    [TestFixture]
    public class ExpressionBuilderTests
    {
        //[Test]
        //public void Test()
        //{
        //    var scheme = new ExpressionScheme<License>();

        //    scheme.Operators
        //          .Simple<Guid>("equals").Register( /*factory*/)
        //          .Simple<string>("equals").Register( /*factory*/)
        //          .Simple<string>("startWith").Register( /*factory*/)
        //          .Simple<string>("endWith").Register( /*factory*/)
        //          .Simple<string>("contains").Register( /*factory*/)
        //          .Simple<IEnumerable>("any").Register( /*factory*/)
        //          .Simple<string>("isUpper").Key("custom").Register( /*factory*/);

        //    scheme.Map
        //          .Route(src => src.Id)
        //          .Route(src => src.Template.Id)
        //          .Route(src => src.Template.Name,
        //                 opt => opt.Operators
        //                           .Reset()
        //                           .Include("custom"))
        //          .Route(src => src.Template.Description);

        //    scheme.Map.Complex(src => src.Claims)
        //          .Route(src => src.Id)
        //          .Route(src => src.Type)
        //          .Route(src => src.Name);

        //    var builder = new TestVisitor(Route.Create("id", "equals", "373474BF-8242-4BE8-88FA-53FFDC3BA20D"));
        //    var licenses = ModelBuilder.CreateModel();
        //    var queryLicenses = licenses.Where(builder.Visit(scheme)).ToList();
        //}
    }
}