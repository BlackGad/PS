using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        #region Test Parser

        //private static MethodInfo GetStringComparisonMethod(string methodName)
        //{
        //    var method = typeof(string).GetMethod("Contains");
        //    switch (methodName.ToLowerInvariant())
        //    {
        //        case "startswith":
        //            method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        //            break;
        //        case FilterOperator.EndsWith:
        //            method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        //            break;
        //        case FilterOperator.Equals:
        //            method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
        //            break;
        //    }
        //    return method;
        //}

        [Test]
        public void Parser()
        {
            var scheme = ExpressionScheme.Create<License>();
            scheme.Converters
                  .Register(Guid.Parse)
                  .Register(int.Parse)
                  .Register(s => s);

            scheme.Operators
                  .Simple<Guid>("equal").Register((src, value) => Expression.Equal(src, Expression.Constant(value)))
                  .Simple<string>("equal").Register((src, value) => Expression.Equal(src, Expression.Constant(value)))
                  .Simple<int>("equal").Register((src, value) => Expression.Equal(src, Expression.Constant(value)))
                  .Complex<int>("count").Register((src, sub) =>
                  {
                      var genericWhereMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                                 .FirstOrDefault(m => m.Name == nameof(Enumerable.Where) &&
                                                                                      m.GetParameters().Length == 2);
                      if (genericWhereMethod == null) throw new InvalidOperationException();

                      var sourceType = src.Type;
                      var itemsType = sourceType.IsGenericType ? sourceType.GetGenericArguments()[0] : typeof(object);

                      var whereMethod = genericWhereMethod.MakeGenericMethod(itemsType);

                      var genericCountMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                                 .FirstOrDefault(m => m.Name == nameof(Enumerable.Count) &&
                                                                                      m.GetParameters().Length == 1);
                      if (genericCountMethod == null) throw new InvalidOperationException();
                      var countMethod = genericCountMethod.MakeGenericMethod(itemsType);

                      Expression whereCall = Expression.Call(whereMethod, src, sub);
                      Expression countCall = Expression.Call(countMethod, whereCall);

                      return countCall;
                  })
                  .Complex<bool>("any").Register((src, sub) =>
                  {
                      var genericAnyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                               .FirstOrDefault(m => m.Name == nameof(Enumerable.Any) &&
                                                                                    m.GetParameters().Length == 2);
                      if (genericAnyMethod == null) throw new InvalidOperationException();

                      var sourceType = src.Type;
                      var itemsType = sourceType.IsGenericType ? sourceType.GetGenericArguments()[0] : typeof(object);

                      var anyMethod = genericAnyMethod.MakeGenericMethod(itemsType);

                      Expression anyCall = Expression.Call(anyMethod, src, sub);
                      return anyCall;
                  })
                  .Simple<string>("contains").Key("custom").Register((src, value) =>
                  {
                      var constant = Expression.Constant(value);
                      BinaryExpression result;
                      if (value == null) result = Expression.Equal(src, constant);
                      else
                      {
                          result = Expression.NotEqual(src, Expression.Constant(null));

                          var method = typeof(string).GetMethod("Contains");
                          if (method == null) throw new InvalidOperationException();

                          result = Expression.AndAlso(result, Expression.Call(src, method, constant));
                      }
                      return result;
                  });

            scheme.Routes
                  .Route(src => src.Id)
                  .Route(src => src.Template.Id)
                  .Route(src => src.Template.Name,
                         opt => opt.Operators
                                   .Reset()
                                   .Include("custom"))
                  .Route(src => src.Template.Description);

            scheme.Routes.Complex(src => src.Claims)
                  .Route(src => src.Id)
                  .Route(src => src.Type)
                  .Route(src => src.Name);

            var json = File.ReadAllText(@"D:\GitHub\PS\PS.Query.Json.Tests\TextFile1.txt");
            var jToken = (JToken)JsonConvert.DeserializeObject(json);
            var provider = new JsonExpressionProvider(jToken);
            var licenses = ModelBuilder.CreateModel();
            var queryLicenses = licenses.Where(scheme.Build(provider)).ToList();
            //PS.Query.Configuration.
        }

        #endregion
    }
}