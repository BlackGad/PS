using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PS.Data.Predicate.Parser
{
    internal class JsonTokenizer
    {
        #region Members

        public JTokenParserToken[] Tokenize(JToken jToken)
        {
            if (jToken == null) throw new ArgumentNullException(nameof(jToken));
            return TokenizeJToken(jToken).Union(new[] { new JTokenParserToken(TokenType.EOS) }).ToArray();
        }

        private IEnumerable<JTokenParserToken> TokenizeJArray(JArray array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            var result = new List<JTokenParserToken>
            {
                new JTokenParserToken(TokenType.ArrayStart)
            };

            foreach (var item in array)
            {
                result.AddRange(TokenizeJToken(item));
            }
            result.Add(new JTokenParserToken(TokenType.ArrayEnd));

            return result;
        }

        private IEnumerable<JTokenParserToken> TokenizeJObject(JObject jObject)
        {
            var result = new List<JTokenParserToken>();
            var properties = jObject.Properties().ToList();
            foreach (var property in properties)
            {
                JTokenParserToken jsonToken;

                if (string.Equals(property.Name, "not", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JTokenParserToken(TokenType.Not);
                else if (properties.Count > 1 || property.Value is JValue)
                    jsonToken = new JTokenParserToken(TokenType.Operator, property.Name);
                else if (string.Equals(property.Name, "and", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JTokenParserToken(TokenType.And);
                else if (string.Equals(property.Name, "or", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JTokenParserToken(TokenType.Or);
                else if (properties.Count == 1 && property.Value is JArray)
                    jsonToken = new JTokenParserToken(TokenType.Operator, property.Name);
                else jsonToken = new JTokenParserToken(TokenType.Object, property.Name);

                result.Add(jsonToken);
                result.AddRange(TokenizeJToken(property.Value));
            }
            return result;
        }

        private IEnumerable<JTokenParserToken> TokenizeJToken(object obj)
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

        private IEnumerable<JTokenParserToken> TokenizeJValue(JValue jObject)
        {
            return new List<JTokenParserToken>
            {
                new JTokenParserToken(TokenType.Value, jObject.ToString(Formatting.None).Trim('\"', '\''))
            };
        }

        #endregion
    }
}