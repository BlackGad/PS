using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using PS.Data.Predicate.Logic;
using PS.Extensions;
using PS.Navigation;
using PS.Threading;

namespace PS.Data.Predicate.Serialization
{
    public static class ExpressionSerialization
    {
        #region Constants

        private const string InvertedOperatorMarker = "NOT ";

        private const string OperatorAttributeName = "Operator";
        private const string QueryAttributeName = "Query";
        private const string RouteAttributeName = "Route";
        private const string ValueAttributeName = "Value";

        private static readonly Dictionary<string, ExpressionTypeCache> CachedExpressionTypes;

        #endregion

        #region Static members

        public static IExpression CreateExpression(string name)
        {
            lock (CachedExpressionTypes)
            {
                return CachedExpressionTypes.Ensure(name, () => new ExpressionTypeCache(name)).CreateInstance();
            }
        }

        public static string GetExpressionName(Type type)
        {
            var elementName = type.Name;
            var xmlRootAttribute = type.GetCustomAttribute<XmlRootAttribute>();
            if (xmlRootAttribute != null) elementName = xmlRootAttribute.ElementName;
            return elementName;
        }

        public static string ReadExpressionOperator(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return reader.GetAttribute(OperatorAttributeName);
        }

        public static Route ReadExpressionRoute(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var routeString = reader.GetAttribute(RouteAttributeName);
            if (string.IsNullOrWhiteSpace(routeString)) return null;
            return Route.Parse(routeString);
        }

        public static IExpression ReadNode(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            var instance = CreateExpression(reader.Name);
            instance.ReadXml(reader);
            return instance;
        }

        public static OperatorExpression ReadOperatorExpression(XmlReader reader)
        {
            var @operator = reader.GetAttribute(OperatorAttributeName);
            var inverted = false;
            if (@operator?.StartsWith(InvertedOperatorMarker) == true)
            {
                inverted = true;
                @operator = @operator.Substring(InvertedOperatorMarker.Length);
            }

            if (string.IsNullOrWhiteSpace(@operator)) return null;

            var value = reader.GetAttribute(ValueAttributeName);
            return new OperatorExpression
            {
                Name = @operator,
                Inverted = inverted,
                Value = value
            };
        }

        public static string ReadSubsetExpressionQuery(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return reader.GetAttribute(QueryAttributeName);
        }

        public static void WriteExpressionOperator(XmlWriter writer, string @operator)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            writer.WriteAttributeString(OperatorAttributeName, @operator);
        }

        public static void WriteExpressionRoute(XmlWriter writer, Route route)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (route == null) throw new ArgumentNullException(nameof(route));
            writer.WriteAttributeString(RouteAttributeName, route.ToString());
        }

        public static void WriteNode(XmlWriter writer, IExpression expression)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var type = expression.GetType();
            var elementName = GetExpressionName(type);
            writer.WriteStartElement(elementName);
            expression.WriteXml(writer);
            writer.WriteEndElement();
        }

        public static void WriteOperatorExpression(XmlWriter writer, OperatorExpression expression)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var operatorName = expression.Name;
            if (expression.Inverted) operatorName = InvertedOperatorMarker + operatorName;
            WriteExpressionOperator(writer, operatorName);
            writer.WriteAttributeString(ValueAttributeName, expression.Value);
        }

        public static void WriteSubsetExpressionQuery(XmlWriter writer, string query)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (string.IsNullOrWhiteSpace(query)) throw new ArgumentNullException(nameof(query));
            writer.WriteAttributeString(QueryAttributeName, query);
        }

        private static void UpdateCachedExpressionTypes(Assembly assembly)
        {
            lock (CachedExpressionTypes)
            {
                var expressionTypes = assembly.GetTypesSafely().Where(t => typeof(IExpression).IsAssignableFrom(t) &&
                                                                           !t.IsAbstract);
                foreach (var expressionType in expressionTypes)
                {
                    var serializationNodeName = GetExpressionName(expressionType);
                    var record = CachedExpressionTypes.Ensure(serializationNodeName, () => new ExpressionTypeCache(serializationNodeName));
                    record.AddType(expressionType);
                }
            }
        }

        #endregion

        #region Constructors

        static ExpressionSerialization()
        {
            CachedExpressionTypes = new Dictionary<string, ExpressionTypeCache>();

            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => UpdateCachedExpressionTypes(args.LoadedAssembly);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                UpdateCachedExpressionTypes(assembly);
            }
        }

        #endregion

        #region Nested type: ExpressionTypeCache

        class ExpressionTypeCache
        {
            private readonly DelegateSwitcher<Func<object>> _factory;
            private readonly string _root;
            private readonly List<Type> _types;

            #region Constructors

            public ExpressionTypeCache(string root)
            {
                if (root == null) throw new ArgumentNullException(nameof(root));
                _root = root;
                _types = new List<Type>();
                _factory = new DelegateSwitcher<Func<object>>();
                _factory.RegisterAndSwitch(nameof(StateDefault), StateDefault);
                _factory.Register(nameof(StateErrorMultipleTypes), StateErrorMultipleTypes);
            }

            #endregion

            #region Members

            public void AddType(Type type)
            {
                var defaultConstructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null,
                    Type.EmptyTypes,
                    null);

                if (defaultConstructor == null) return;

                if (_types.Any()) _factory.Switch(nameof(StateErrorMultipleTypes));
                else
                {
                    if (defaultConstructor.IsPublic)
                    {
                        var lambda = Expression.Lambda<Func<object>>(Expression.New(type)).Compile();
                        _factory.RegisterAndSwitch(string.Empty, lambda);
                    }
                    else
                    {
                        _factory.RegisterAndSwitch(string.Empty, () => defaultConstructor.Invoke(null));
                    }
                }
                _types.Add(type);
            }

            public IExpression CreateInstance()
            {
                return (IExpression)_factory.Active();
            }

            private object StateDefault()
            {
                throw new InvalidOperationException($"Ambiguous type for '{_root}' node");
            }

            private object StateErrorMultipleTypes()
            {
                throw new InvalidOperationException($"Ambiguous type for '{_root}' node. " +
                                                    $"Possible types: {string.Join(", ", _types.Select(t => t.Name))}");
            }

            #endregion
        }

        #endregion
    }
}