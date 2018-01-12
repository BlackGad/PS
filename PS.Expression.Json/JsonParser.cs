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

            ctx.Sequence("EXPRESSION_CONDITION eos")
               .Assert(EXPRESSION_CONDITION)
               .Assert((t, env) => t.Type == TokenType.EOS);

            var dot = ctx.ToString("d");
            return tokens;
        }

        protected bool ProcessOperator(JsonToken t, ParseEnvironment env)
        {
            if (t.Type != TokenType.Operator) return false;
            var operatorSession = env[Keys.RouteSession] + "_operator";
            env[operatorSession] = t.Value;

            //var isValidOperator = _scheme.ProcessOperator(contextProperty, t.Value);
            //if (isValidOperator)
            //{
            //}
            //return isValidOperator;
            return true;
        }

        protected bool ProcessRoute(JsonToken t, ParseEnvironment env)
        {
            if (t.Type != TokenType.Object) return false;
            var routeSession = env[Keys.RouteSession] + "_route";
            env[routeSession] = Route.Create(env[routeSession], t.Value);
            return true;
        }

        protected void ProcessRouteSession(ParseEnvironment env)
        {
            env[Keys.RouteSession] = Guid.NewGuid().ToString("N");
        }

        private void EXPRESSION(ParseContext ctx)
        {
            ctx.Sequence("object(route) ROUTE_LIST OPERATION")
               .Assert(ProcessRoute)
               .Assert(ROUTE_LIST)
               .Assert(OPERATION);

            ctx.Sequence("object(route) ROUTE_LIST operator EXPRESSION_CONDITION")
               .Assert(ProcessRoute)
               .Assert(ROUTE_LIST)
               .Assert(ProcessOperator)
               .Assert(EXPRESSION_CONDITION);

            ctx.Sequence("object(route) ROUTE_LIST operator EXPRESSION_CONDITION OPERATION")
               .Assert(ProcessRoute)
               .Assert(ROUTE_LIST)
               .Assert(ProcessOperator)
               .Assert(EXPRESSION_CONDITION)
               .Assert(OPERATION);
        }

        private void EXPRESSION_CONDITION(ParseContext ctx)
        {
            ctx.Sequence("and EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.And)
               .Assert(EXPRESSION_CONDITION_BODY);

            ctx.Sequence("or EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.Or)
               .Assert(EXPRESSION_CONDITION_BODY);

            ctx.Sequence("EXPRESSION_CONDITION_BODY")
               .Assert(EXPRESSION_CONDITION_BODY);
        }

        private void EXPRESSION_CONDITION_BODY(ParseContext ctx)
        {
            ctx.Sequence("ArrayStart EXPRESSION_LIST ArrayEnd")
               .Assert((t, env) => t.Type == TokenType.ArrayStart)
               .Assert(EXPRESSION_LIST)
               .Assert((t, env) => t.Type == TokenType.ArrayEnd);
        }

        private void EXPRESSION_LIST(ParseContext ctx)
        {
            var id = Guid.NewGuid();
            ctx.Sequence("EXPRESSION EXPRESSION_LIST")
               .Assert("(M)Route session", ProcessRouteSession)
               .Assert(EXPRESSION)
               .Assert(EXPRESSION_LIST);

            ctx.Sequence("EMPTY")
               .Assert();
        }

        private void OPERATION(ParseContext ctx)
        {
            ctx.Sequence("not operator value")
               .Assert((t, env) => t.Type == TokenType.Not)
               .Assert(ProcessOperator)
               .Assert((t, env) => t.Type == TokenType.Value);

            ctx.Sequence("operator value")
               .Assert(ProcessOperator)
               .Assert((t, env) => t.Type == TokenType.Value);
        }

        private void ROUTE_LIST(ParseContext ctx)
        {
            ctx.Sequence("object(route) ROUTE_LIST")
               .Assert(ProcessRoute)
               .Assert(ROUTE_LIST);

            ctx.Sequence("EMPTY")
               .Assert();
        }

        #endregion

        #region Nested type: Keys

        class Keys
        {
            #region Constants

            public const string Operator = nameof(Operator);
            //public const string Route = nameof(Route);
            public const string RouteSession = nameof(RouteSession);
            public const string RouteSessionList = nameof(RouteSessionList);
            public const string Value = nameof(Value);

            #endregion
        }

        #endregion
    }
}