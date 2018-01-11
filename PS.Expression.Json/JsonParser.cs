using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PS.Expression.Test2;
using PS.Navigation;

namespace PS.Expression.Json
{
    /// <summary>
    ///     https://jack-vanlightly.com/blog/2016/2/11/implementing-a-dsl-parser
    /// </summary>
    /// <remarks>
    ///     Grammar:
    ///     S -> EXPRESSION_CONDITION
    ///     EXPRESSION_CONDITION -> object(and) EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION -> object(or) EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION -> EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION_BODY -> ArrayStart EXPRESSION_LIST ArrayEnd
    ///     EXPRESSION_LIST -> EXPRESSION EXPRESSION_LIST
    ///     EXPRESSION_LIST -> EXPRESSION
    ///     EXPRESSION -> object(property) EXPRESSION
    ///     EXPRESSION -> object(not) object(operator) value
    ///     EXPRESSION -> object(operator) value
    /// </remarks>
    public class JsonParser<T>
    {
        private readonly ExpressionScheme<T> _scheme;

        #region Constructors

        public JsonParser(ExpressionScheme<T> scheme)
        {
            if (scheme == null) throw new ArgumentNullException(nameof(scheme));
            _scheme = scheme;
        }

        #endregion

        #region Members

        public JsonToken[] Parse(string json)
        {
            var tokenizer = new JsonTokenizer();
            var tokens = tokenizer.Tokenize((JToken)JsonConvert.DeserializeObject(json));

            var ctx = new ParseContext(tokens);
            ctx.Environment.Add(nameof(ExpressionSchemeRoute.Route), Route.Create());

            ctx.CheckSequence("EXPRESSION_CONDITION eos")
               .Assert(EXPRESSION_CONDITION)
               .Assert((t, env) => t.Type == TokenType.EOS);
            var dot = ctx.ToString("d");
            return tokens;
        }

        private void EXPRESSION(ParseContext ctx)
        {
            ctx.CheckSequence("object(property) EXPRESSION")
               .Assert((t, env) =>
               {
                   if (t.Type != TokenType.Object) return false;

                   var parentProperty = (Route)ctx.Environment[nameof(ExpressionSchemeRoute.Route)];
                   var currentRoutePart = Route.Create(parentProperty, t.Value);
                   var isValidProperty = _scheme.Map.IsValidRoute(currentRoutePart);
                   if (isValidProperty) ctx.Environment[nameof(ExpressionSchemeRoute.Route)] = currentRoutePart;

                   return isValidProperty;
               })
               .Assert(EXPRESSION);

            ctx.CheckSequence("object(not) object(operator) value")
               .Assert((t, env) => t.Type == TokenType.Object && string.Equals(t.Value, "not", StringComparison.InvariantCultureIgnoreCase))
               .Assert((t, env) => t.Type == TokenType.Object && string.Equals(t.Value, "equal", StringComparison.InvariantCultureIgnoreCase))
               .Assert((t, env) => t.Type == TokenType.Value);

            ctx.CheckSequence("object(operator) value")
               .Assert((t, env) => t.Type == TokenType.Object && string.Equals(t.Value, "equal", StringComparison.InvariantCultureIgnoreCase))
               .Assert((t, env) => t.Type == TokenType.Value);
        }

        private void EXPRESSION_CONDITION(ParseContext ctx)
        {
            ctx.CheckSequence("object(and) EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.Object && string.Equals(t.Value, "and", StringComparison.InvariantCultureIgnoreCase))
               .Assert(EXPRESSION_CONDITION_BODY);

            ctx.CheckSequence("object(or) EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.Object && string.Equals(t.Value, "or", StringComparison.InvariantCultureIgnoreCase))
               .Assert(EXPRESSION_CONDITION_BODY);

            ctx.CheckSequence("EXPRESSION_CONDITION_BODY")
               .Assert(EXPRESSION_CONDITION_BODY);
        }

        private void EXPRESSION_CONDITION_BODY(ParseContext ctx)
        {
            ctx.CheckSequence("ArrayStart EXPRESSION_LIST ArrayEnd")
               .Assert((t, env) => t.Type == TokenType.ArrayStart)
               .Assert(EXPRESSION_LIST)
               .Assert((t, env) => t.Type == TokenType.ArrayEnd);
        }

        private void EXPRESSION_LIST(ParseContext ctx)
        {
            ctx.CheckSequence("EXPRESSION EXPRESSION_LIST")
               .Assert(EXPRESSION)
               .Assert(EXPRESSION_LIST);

            ctx.CheckSequence("EXPRESSION")
               .Assert(EXPRESSION);
        }

        #endregion
    }
}