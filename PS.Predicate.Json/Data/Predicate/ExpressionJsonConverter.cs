using System;
using Newtonsoft.Json;
using PS.Data.Predicate.Logic;
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
                    WriteJson(writer, expression, serializer);
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
                return;
            }

            var routeExpression = value as RouteExpression;
            if (routeExpression != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(routeExpression.Route.ToString().ToLowerInvariant());
                writer.WriteStartObject();
                WriteExpressionOperator(writer, serializer, routeExpression.Operator);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            var operatorExpression = value as OperatorExpression;
            if (operatorExpression != null)
            {
                if (operatorExpression.Inverted)
                {
                    
                }
            }
            
        }

        private void WriteExpressionOperator(JsonWriter writer, JsonSerializer serializer, OperatorExpression @operator)
        {
            if (@operator == null) return;

            if (@operator.Inverted)
            {
                writer.WritePropertyName("not");
            }
        }

        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        ///     The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IExpression).IsAssignableFrom(objectType);
        }

        #endregion
    }
}