using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PS.Query.Json
{
    class JsonTokenizer
    {
        #region Members

        public JsonToken[] Tokenize(JToken jToken)
        {
            if (jToken == null) throw new ArgumentNullException(nameof(jToken));
            return TokenizeJToken(jToken).Union(new[] { new JsonToken(TokenType.EOS) }).ToArray();
        }

        private IEnumerable<JsonToken> TokenizeJArray(JArray array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            var result = new List<JsonToken>
            {
                new JsonToken(TokenType.ArrayStart)
            };

            foreach (var item in array)
            {
                result.AddRange(TokenizeJToken(item));
            }
            result.Add(new JsonToken(TokenType.ArrayEnd));

            return result;
        }

        private IEnumerable<JsonToken> TokenizeJObject(JObject jObject)
        {
            var result = new List<JsonToken>();
            var properties = jObject.Properties().ToList();
            foreach (var property in properties)
            {
                JsonToken jsonToken;

                if (string.Equals(property.Name, "not", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JsonToken(TokenType.Not);
                else if (properties.Count > 1 || property.Value is JValue)
                    jsonToken = new JsonToken(TokenType.Operator, property.Name);
                else if (string.Equals(property.Name, "and", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JsonToken(TokenType.And);
                else if (string.Equals(property.Name, "or", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JsonToken(TokenType.Or);
                else if (properties.Count == 1 && property.Value is JArray)
                    jsonToken = new JsonToken(TokenType.Operator, property.Name);
                else jsonToken = new JsonToken(TokenType.Object, property.Name);

                result.Add(jsonToken);
                result.AddRange(TokenizeJToken(property.Value));
            }
            return result;
        }

        private IEnumerable<JsonToken> TokenizeJToken(object obj)
        {
            var token = obj as JToken;
            if (token == null) throw new InvalidOperationException();

            var jArray = obj as JArray;
            if (jArray != null) return TokenizeJArray(jArray);

            var jObject = obj as JObject;
            if (jObject != null) return TokenizeJObject(jObject);

            var jValue = obj as JValue;
            if (jValue != null) return TokenizeJValue(jValue);

            throw new NotSupportedException();
        }

        private IEnumerable<JsonToken> TokenizeJValue(JValue jObject)
        {
            return new List<JsonToken>
            {
                new JsonToken(TokenType.Value, jObject.ToString(Formatting.None))
            };
        }

        #endregion
    }
}