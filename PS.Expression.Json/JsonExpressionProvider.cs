using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using PS.Navigation;
using PS.Query.Logic;
using PS.Query.Model;
using LogicalExpression = PS.Query.Model.LogicalExpression;

namespace PS.Query.Json
{
    /// <summary>
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
    /// </summary>
    public class JsonExpressionProvider : IExpressionProvider
    {
        private readonly JsonToken[] _tokens;

        #region Constructors

        public JsonExpressionProvider(JToken jToken)
        {
            if (jToken == null) throw new ArgumentNullException(nameof(jToken));
            var tokenizer = new JsonTokenizer();
            _tokens = tokenizer.Tokenize(jToken);
        }

        #endregion

        #region IExpressionProvider Members

        public LogicalExpression Provide()
        {
            var ctx = new ParseContext(_tokens);

            ctx.Sequence("EXPRESSION_CONDITION eos")
               .Assert(EXPRESSION_CONDITION)
               .Assert((t, env) => t.Type == TokenType.EOS);

            if (ctx.FailedBranch != null) throw ctx.FailedBranch.Error;
            if (ctx.SuccessBranch != null) return ctx.SuccessBranch.Environment.Get<LogicalExpression>();
            throw new InvalidOperationException();
        }

        #endregion

        #region Members

        protected bool AggregateContextRoute(JsonToken t, ParseEnvironment env)
        {
            if (t.Type != TokenType.Object) return false;
            var contextRoute = env.Get<RouteExpression>();
            contextRoute.Route = Route.Create(contextRoute.Route, t.Value);

            return true;
        }

        protected bool CheckToken(JsonToken t, TokenType type, Action action)
        {
            if (t.Type != type) return false;
            action();
            return true;
        }

        protected void CommitContextRoute(ParseEnvironment env)
        {
            var routeList = env.Get<RouteExpression[]>();
            env.Set(routeList.Union(new[] { env.Pop<RouteExpression>() }).ToArray());
        }

        protected void RestoreContextRoute(ParseEnvironment env)
        {
            var snapshot = (Tuple<RouteExpression[], RouteExpression>)env[env.Id];
            ((ComplexRouteExpression)snapshot.Item2).Sub = env.Get<LogicalExpression>();

            env.Set(snapshot.Item1);
            env.Set(snapshot.Item2);
            env[env.Id] = null;
        }

        protected void SaveContextRoute(ParseEnvironment env)
        {
            env[env.Id] = new Tuple<RouteExpression[], RouteExpression>(env.Pop<RouteExpression[]>(), env.Pop<RouteExpression>());
        }

        private void EXPRESSION(ParseContext ctx)
        {
            ctx.Sequence("object(route) ROUTE_LIST OPERATION")
               .Assert("(M)Create route", env => env.Set(new RouteExpression()))
               .Assert(AggregateContextRoute)
               .Assert(ROUTE_LIST)
               .Assert(OPERATION)
               .Assert("(M)Commit route", CommitContextRoute);

            ctx.Sequence("object(route) ROUTE_LIST operator EXPRESSION_CONDITION")
               .Assert("(M)Create route", env => env.Set<RouteExpression>(new ComplexRouteExpression()))
               .Assert(AggregateContextRoute)
               .Assert(ROUTE_LIST)
               .Assert((token, env) => CheckToken(token,
                                                  TokenType.Operator,
                                                  () => ((ComplexRouteExpression)env.Get<RouteExpression>()).ComplexOperator = token.Value))
               .Assert("(M)Push route", SaveContextRoute)
               .Assert(EXPRESSION_CONDITION)
               .Assert("(M)Pop route", RestoreContextRoute)
               .Assert("(M)Commit route", CommitContextRoute);

            ctx.Sequence("object(route) ROUTE_LIST operator EXPRESSION_CONDITION OPERATION")
               .Assert("(M)Create route", env => env.Set<RouteExpression>(new ComplexRouteExpression()))
               .Assert(AggregateContextRoute)
               .Assert(ROUTE_LIST)
               .Assert((token, env) => CheckToken(token,
                                                  TokenType.Operator,
                                                  () => ((ComplexRouteExpression)env.Get<RouteExpression>()).ComplexOperator = token.Value))
               .Assert("(M)Save route", SaveContextRoute)
               .Assert(EXPRESSION_CONDITION)
               .Assert("(M)Pop route", RestoreContextRoute)
               .Assert(OPERATION)
               .Assert("(M)Commit route", CommitContextRoute);
        }

        private void EXPRESSION_CONDITION(ParseContext ctx)
        {
            ctx.Sequence("and EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.And)
               .Assert(EXPRESSION_CONDITION_BODY)
               .Assert("(M)Commit AND expression", env => env.Set(new LogicalExpression(LogicalOperator.And, env.Get<RouteExpression[]>())));

            ctx.Sequence("or EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.Or)
               .Assert(EXPRESSION_CONDITION_BODY)
               .Assert("(M)Commit OR expression", env => env.Set(new LogicalExpression(LogicalOperator.Or, env.Get<RouteExpression[]>())));

            ctx.Sequence("EXPRESSION_CONDITION_BODY")
               .Assert(EXPRESSION_CONDITION_BODY)
               .Assert("(M)Commit AND expression", env => env.Set(new LogicalExpression(LogicalOperator.And, env.Get<RouteExpression[]>())));
        }

        private void EXPRESSION_CONDITION_BODY(ParseContext ctx)
        {
            ctx.Sequence("ArrayStart EXPRESSION_LIST ArrayEnd")
               .Assert((t, env) => t.Type == TokenType.ArrayStart)
               .Assert("(M)Create route list", env => env.Set(new RouteExpression[] { }))
               .Assert(EXPRESSION_LIST)
               .Assert((t, env) => t.Type == TokenType.ArrayEnd);
        }

        private void EXPRESSION_LIST(ParseContext ctx)
        {
            ctx.Sequence("EXPRESSION EXPRESSION_LIST")
               .Assert(EXPRESSION)
               .Assert(EXPRESSION_LIST);

            ctx.Sequence("EMPTY")
               .Assert();
        }

        private void OPERATION(ParseContext ctx)
        {
            ctx.Sequence("not operator value")
               .Assert("(M)Create operator", env => env.Set(new OperatorExpression()))
               .Assert((t, env) => CheckToken(t, TokenType.Not, () => env.Get<OperatorExpression>().Inverted = true))
               .Assert((t, env) => CheckToken(t, TokenType.Operator, () => env.Get<OperatorExpression>().Operator = t.Value))
               .Assert((t, env) => CheckToken(t, TokenType.Value, () => env.Get<OperatorExpression>().Value = t.Value))
               .Assert("(M)Commit operator", env => env.Get<RouteExpression>().Operator = env.Get<OperatorExpression>());

            ctx.Sequence("operator value")
               .Assert("(M)Create operator", env => env.Set(new OperatorExpression()))
               .Assert((t, env) => CheckToken(t, TokenType.Operator, () => env.Get<OperatorExpression>().Operator = t.Value))
               .Assert((t, env) => CheckToken(t, TokenType.Value, () => env.Get<OperatorExpression>().Value = t.Value))
               .Assert("(M)Commit operator", env => env.Get<RouteExpression>().Operator = env.Get<OperatorExpression>());
        }

        private void ROUTE_LIST(ParseContext ctx)
        {
            ctx.Sequence("object(route) ROUTE_LIST")
               .Assert(AggregateContextRoute)
               .Assert(ROUTE_LIST);

            ctx.Sequence("EMPTY")
               .Assert();
        }

        #endregion
    }
}