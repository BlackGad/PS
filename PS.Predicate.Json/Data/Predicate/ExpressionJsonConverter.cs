using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PS.Data.Predicate.Logic;
using PS.Data.Predicate.Parser;
using PS.Extensions;

namespace PS.Data.Predicate
{
    public class ExpressionJsonConverter : JsonConverter
    {
        #region Override members

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var logicalExpression = value as LogicalExpression;
            if (logicalExpression != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(logicalExpression.Operator.ToString().ToLowerInvariant());
                writer.WriteStartArray();
                foreach (var expression in logicalExpression.Expressions.Enumerate())
                {
                    serializer.Serialize(writer, expression);
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
                return;
            }

            var routeSubsetExpression = value as RouteSubsetExpression;
            if (routeSubsetExpression != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(routeSubsetExpression.Route.ToString().ToLowerInvariant());
                writer.WriteStartObject();
                writer.WritePropertyName(routeSubsetExpression.Query.ToLowerInvariant());
                serializer.Serialize(writer, routeSubsetExpression.Subset);
                if (routeSubsetExpression.Operator != null)
                {
                    WriteJson(writer, routeSubsetExpression.Operator, serializer);
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
                return;
            }

            var routeExpression = value as RouteExpression;
            if (routeExpression != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(routeExpression.Route.ToString().ToLowerInvariant());
                writer.WriteStartObject();
                WriteJson(writer, routeExpression.Operator, serializer);
                writer.WriteEndObject();
                writer.WriteEndObject();
                return;
            }

            var operatorExpression = value as OperatorExpression;
            if (operatorExpression != null)
            {
                if (operatorExpression.Inverted)
                {
                    writer.WritePropertyName("not");
                    writer.WriteStartObject();
                }

                writer.WritePropertyName(operatorExpression.Name.ToLowerInvariant());
                writer.WriteValue(operatorExpression.Value);

                if (operatorExpression.Inverted) writer.WriteEndObject();
                return;
            }

            throw new NotSupportedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jToken = (JToken)serializer.Deserialize(reader);
            var parser = new JTokenParser(jToken);
            return parser.Parse();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IExpression).IsAssignableFrom(objectType);
        }

        #endregion
    }
}