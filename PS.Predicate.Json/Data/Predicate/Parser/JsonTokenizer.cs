using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PS.Data.Predicate.Parser
{
    internal class JsonTokenizer
    {
        private List<JTokenParserToken> _tokens;

        #region Members

        public JTokenParserToken[] Tokenize(JToken jToken)
        {
            if (jToken == null) throw new ArgumentNullException(nameof(jToken));
            _tokens = new List<JTokenParserToken>();
            TokenizeJToken(jToken);
            _tokens.Add(new JTokenParserToken(TokenType.EOS));
            return _tokens.ToArray();
        }

        private void TokenizeJArray(JArray array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            _tokens.Add(new JTokenParserToken(TokenType.ArrayStart));

            foreach (var item in array)
            {
                TokenizeJToken(item);
            }

            _tokens.Add(new JTokenParserToken(TokenType.ArrayEnd));
        }

        private void TokenizeJObject(JObject jObject)
        {
            var properties = jObject.Properties().ToList();
            foreach (var property in properties)
            {
                JTokenParserToken jsonToken;

                if (string.Equals(property.Name, "not", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JTokenParserToken(TokenType.Not);
                else if (string.Equals(property.Name, "and", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JTokenParserToken(TokenType.And);
                else if (string.Equals(property.Name, "or", StringComparison.InvariantCultureIgnoreCase))
                    jsonToken = new JTokenParserToken(TokenType.Or);
                else if (properties.Count > 1 || property.Value is JValue)
                    jsonToken = new JTokenParserToken(TokenType.Operator, property.Name);
                else if (properties.Count == 1 && property.Value is JArray)
                    jsonToken = new JTokenParserToken(TokenType.Operator, property.Name);
                else
                {
                    jsonToken = _tokens.LastOrDefault()?.Type == TokenType.Object
                        ? new JTokenParserToken(TokenType.Operator, property.Name)
                        : new JTokenParserToken(TokenType.Object, property.Name);
                }

                _tokens.Add(jsonToken);

                TokenizeJToken(property.Value);
            }
        }

        private void TokenizeJToken(object obj)
        {
            var token = obj as JToken;
            if (token == null) throw new InvalidOperationException();

            var jArray = obj as JArray;
            if (jArray != null)
            {
                TokenizeJArray(jArray);
                return;
            }

            var jObject = obj as JObject;
            if (jObject != null)
            {
                TokenizeJObject(jObject);
                return;
            }

            var jValue = obj as JValue;
            if (jValue != null)
            {
                TokenizeJValue(jValue);
                return;
            }

            throw new NotSupportedException();
        }

        private void TokenizeJValue(JValue jObject)
        {
            _tokens.Add(new JTokenParserToken(TokenType.Value, jObject.ToString(Formatting.None).Trim('\"', '\'')));
        }

        #endregion
    }
}