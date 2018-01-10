using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PS.Expression.Json
{
    public class JsonTokenizer
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
                result.Add(new JsonToken(TokenType.Object, property.Name));
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