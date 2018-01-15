//using System;
//using System.Linq;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using PS.Expression.Extensions;
//using PS.Expression.Logic;
//using PS.Expression.Logic.Extensions;
//using PS.Expression.Test1;
//using PS.Expression.Test1.Logic;

//namespace PS.Expression.Json
//{
//    public class JsonExpressionVisitor
//    {
//        #region Static members

//        public static ILogicalExpression Parse(string json)
//        {
//            //var visitor = new JsonExpressionVisitor()
//            return JsonConvert.DeserializeObject(json)
//                              .Factorize(new FactorizeParams((o, inverted) => ParseJToken(null, o, inverted)));
//        }

//        private static object ParseJArray(string key, JArray array, bool inverted)
//        {
//            if (array == null) throw new ArgumentNullException(nameof(array));

//            if (string.Equals(key, "and", StringComparison.InvariantCultureIgnoreCase) || key == null)
//            {
//                var logicalExpression = new LogicalExpression(LogicalOperator.And);
//                foreach (var item in array)
//                {
//                    logicalExpression.AddExpression(ParseJToken(null, item, inverted));
//                }

//                return logicalExpression;
//            }

//            if (string.Equals(key, "or", StringComparison.InvariantCultureIgnoreCase))
//            {
//                var logicalExpression = new LogicalExpression(LogicalOperator.Or);
//                foreach (var item in array)
//                {
//                    logicalExpression.AddExpression(ParseJToken(null, item, inverted));
//                }

//                return logicalExpression;
//            }

//            var result = new GroupedExpression(key);
//            foreach (var item in array)
//            {
//                result.AddExpression(ParseJToken(null, item, inverted));
//            }
//            return result;
//        }

//        private static object ParseJObject(string key, JObject jObject, bool inverted)
//        {
//            var properties = jObject.Properties().ToList();
//            if (properties.Count != 1) throw new FormatException("JSON format is invalid");

//            var property = properties.First();
//            var followedExpression = ParseJToken(property.Name, property.Value, inverted);

//            if (key == null) return new HeadExpression(followedExpression);

//            if (string.Equals(key, "not", StringComparison.InvariantCultureIgnoreCase)) return new InvertedLogicalExpression(followedExpression);

//            return new InterExpression(key, followedExpression);
//        }

//        private static object ParseJToken(string key, object obj, bool inverted)
//        {
//            var token = obj as JToken;
//            if (token == null) return obj;

//            var jArray = obj as JArray;
//            if (jArray != null) return ParseJArray(key, jArray, inverted);

//            var jObject = obj as JObject;
//            if (jObject != null) return ParseJObject(key, jObject, inverted);

//            var jValue = obj as JValue;
//            if (jValue != null) return ParseJValue(key, jValue, inverted);

//            throw new NotSupportedException();
//        }

//        private static object ParseJValue(string key, JValue jObject, bool inverted)
//        {
//            return new InterExpression(key, new ValueExpression(jObject.ToString(Formatting.None)));
//        }

//        #endregion

//        #region Constructors

//        public JsonExpressionVisitor() 
//        {
//        }

//        #endregion
//    }
//}