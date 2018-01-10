using System;
using PS.Expression.Test2;
using PS.Expression.Tests.TestReferences.ExpressionBuilderTests.Model;
using PS.Navigation;

namespace PS.Expression.Tests.TestReferences.ExpressionBuilderTests
{
    class TestVisitor : ExpressionVisitor<License>
    {
        private readonly Route _route;

        #region Constructors

        public TestVisitor(Route route)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));
            _route = route;
        }

        #endregion

        #region Override members

        /// <summary>
        /// </summary>
        /// <param name="scheme"></param>
        /// <example>
        ///     id equal FA36C38F-C1FF-4740-8F1E-5D0D5D76E508
        /// </example>
        /// <returns></returns>
        public override Func<License, bool> Visit(ExpressionScheme<License> scheme)
        {
            var context = scheme.CreateReaderContext();

            foreach (var path in _route)
            {
            }

            var param = System.Linq.Expressions.Expression.Parameter(typeof(License));
            var property = System.Linq.Expressions.Expression.Property(param, "Id");
            var value = System.Linq.Expressions.Expression.Constant(Guid.NewGuid());
            var comparison = System.Linq.Expressions.Expression.Equal(property, value);
            //context.
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<License, bool>>(comparison, param);
            return lambda.Compile();
        }

        #endregion

        /*
         
ParameterExpression argParam = Expression.Parameter(typeof(Service), "s");
Expression nameProperty = Expression.Property(argParam, "Name");
Expression namespaceProperty = Expression.Property(argParam, "Namespace");

var val1 = Expression.Constant("Modules");
var val2 = Expression.Constant("Namespace");

Expression e1 = Expression.Equal(nameProperty, val1);
Expression e2 = Expression.Equal(namespaceProperty, val2);
var andExp = Expression.AndAlso(e1, e2);

var lambda = Expression.Lambda<Func<Service, bool>>(andExp, argParam);
         */
    }

    class MyClass
    {
        
    }
}