using System;
using System.Collections.Generic;
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
    ///     S -> EXPRESSION_CONDITION eos
    ///     EXPRESSION_CONDITION -> and EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION -> or EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION -> EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION_BODY -> ArrayStart EXPRESSION_LIST ArrayEnd
    ///     EXPRESSION_LIST -> EXPRESSION EXPRESSION_LIST
    ///     EXPRESSION_LIST -> EMPTY
    ///     EXPRESSION -> object(route) ROUTE_LIST OPERATION
    ///     EXPRESSION -> object(route) ROUTE_LIST operator EXPRESSION_CONDITION
    ///     EXPRESSION -> object(route) ROUTE_LIST operator EXPRESSION_CONDITION OPERATION
    ///     ROUTE_LIST -> object(route) ROUTE_LIST
    ///     ROUTE_LIST -> EMPTY
    ///     OPERATION -> not operator value
    ///     OPERATION -> operator value
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
            ctx.CheckSequence("object(route) ROUTE_LIST OPERATION")
               .Assert(IsValidRoute)
               .Assert(ROUTE_LIST)
               .Assert(OPERATION);

            ctx.CheckSequence("object(route) ROUTE_LIST operator EXPRESSION_CONDITION")
               .Assert(IsValidRoute)
               .Assert(ROUTE_LIST)
               .Assert(IsValidOperator)
               .Assert(EXPRESSION_CONDITION);

            ctx.CheckSequence("object(route) ROUTE_LIST operator EXPRESSION_CONDITION OPERATION")
               .Assert(IsValidRoute)
               .Assert(ROUTE_LIST)
               .Assert(IsValidOperator)
               .Assert(EXPRESSION_CONDITION)
               .Assert(OPERATION);
        }

        private void EXPRESSION_CONDITION(ParseContext ctx)
        {
            ctx.CheckSequence("and EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.And)
               .Assert(EXPRESSION_CONDITION_BODY);

            ctx.CheckSequence("or EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.Or)
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

            ctx.CheckSequence("EMPTY")
               .AssertEmpty();
        }

        private bool IsValidOperator(JsonToken t, Dictionary<object, object> env)
        {
            if (t.Type != TokenType.Operator) return false;
            var contextProperty = (Route)env[nameof(ExpressionSchemeRoute.Route)];
            //var isValidOperator = _scheme.IsValidOperator(contextProperty, t.Value);
            //if (isValidOperator)
            //{
            //}
            //return isValidOperator;
            return true;
        }

        private bool IsValidRoute(JsonToken t, Dictionary<object, object> env)
        {
            if (t.Type != TokenType.Object) return false;

            var contextProperty = (Route)env[nameof(ExpressionSchemeRoute.Route)];
            var currentRoutePart = Route.Create(contextProperty, t.Value);
            //var isValidProperty = _scheme.Map.IsValidRoute(currentRoutePart);
            //if (isValidProperty) 
            env[nameof(ExpressionSchemeRoute.Route)] = currentRoutePart;
            //return isValidProperty;
            return true;
        }

        private void OPERATION(ParseContext ctx)
        {
            ctx.CheckSequence("not operator value")
               .Assert((t, env) => t.Type == TokenType.Not)
               .Assert(IsValidOperator)
               .Assert((t, env) => t.Type == TokenType.Value);

            ctx.CheckSequence("operator value")
               .Assert(IsValidOperator)
               .Assert((t, env) => t.Type == TokenType.Value);
        }

        private void ROUTE_LIST(ParseContext ctx)
        {
            ctx.CheckSequence("object(route) ROUTE_LIST")
               .Assert(IsValidRoute)
               .Assert(ROUTE_LIST);

            ctx.CheckSequence("EMPTY")
               .AssertEmpty();
        }

        #endregion
    }
}