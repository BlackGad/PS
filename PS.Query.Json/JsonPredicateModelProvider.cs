using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using PS.Data.Logic;
using PS.Navigation;
using PS.Parser;
using PS.Query.Data.Predicate;
using PS.Query.Data.Predicate.Logic;
using LogicalExpression = PS.Query.Data.Predicate.Logic.LogicalExpression;

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
    public class JsonPredicateModelProvider : IPredicateModelProvider
    {
        private readonly JsonToken[] _tokens;

        #region Constructors

        public JsonPredicateModelProvider(JToken jToken)
        {
            if (jToken == null) throw new ArgumentNullException(nameof(jToken));
            var tokenizer = new JsonTokenizer();
            _tokens = tokenizer.Tokenize(jToken);
        }

        #endregion

        #region IPredicateModelProvider Members

        public LogicalExpression Provide()
        {
            var ctx = ParseContext<JsonToken>.Parse(_tokens);

            ctx.Sequence("EXPRESSION_CONDITION eos")
               .Assert(EXPRESSION_CONDITION)
               .Assert((t, env) => t.Type == TokenType.EOS);

            if (ctx.FailedBranch != null) throw ctx.FailedBranch.Error;
            if (ctx.SuccessBranch != null) return ctx.SuccessBranch.Environment.Get<LogicalExpression>();
            throw new InvalidOperationException();
        }

        #endregion

        #region Members

        internal bool AggregateContextRoute(JsonToken t, ParseEnvironment env)
        {
            if (t.Type != TokenType.Object) return false;
            var contextRoute = env.Get<RouteExpression>();
            contextRoute.Route = Route.Create(contextRoute.Route, t.Value);

            return true;
        }

        internal bool CheckToken(JsonToken t, TokenType type, Action action)
        {
            if (t.Type != type) return false;
            action();
            return true;
        }

        internal void CommitContextRoute(ParseEnvironment env)
        {
            var routeList = env.Get<RouteExpression[]>();
            env.Set(routeList.Union(new[] { env.Pop<RouteExpression>() }).ToArray());
        }

        internal void RestoreContextRoute(ParseEnvironment env)
        {
            var snapshot = (Tuple<RouteExpression[], RouteExpression>)env[env.Id];
            ((RouteSubsetExpression)snapshot.Item2).Sub = env.Get<LogicalExpression>();

            env.Set(snapshot.Item1);
            env.Set(snapshot.Item2);
            env[env.Id] = null;
        }

        internal void SaveContextRoute(ParseEnvironment env)
        {
            env[env.Id] = new Tuple<RouteExpression[], RouteExpression>(env.Pop<RouteExpression[]>(), env.Pop<RouteExpression>());
        }

        private void EXPRESSION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("object(route) ROUTE_LIST OPERATION")
               .Assert("Create route", env => env.Set(new RouteExpression()))
               .Assert(AggregateContextRoute)
               .Assert(ROUTE_LIST)
               .Assert(OPERATION)
               .Assert("Commit route", CommitContextRoute);

            ctx.Sequence("object(route) ROUTE_LIST operator EXPRESSION_CONDITION")
               .Assert("Create route", env => env.Set<RouteExpression>(new RouteSubsetExpression()))
               .Assert(AggregateContextRoute)
               .Assert(ROUTE_LIST)
               .Assert((token, env) => CheckToken(token,
                                                  TokenType.Operator,
                                                  () => ((RouteSubsetExpression)env.Get<RouteExpression>()).SubsetOperator = token.Value))
               .Assert("Push route", SaveContextRoute)
               .Assert(EXPRESSION_CONDITION)
               .Assert("Pop route", RestoreContextRoute)
               .Assert("Commit route", CommitContextRoute);

            ctx.Sequence("object(route) ROUTE_LIST operator EXPRESSION_CONDITION OPERATION")
               .Assert("Create route", env => env.Set<RouteExpression>(new RouteSubsetExpression()))
               .Assert(AggregateContextRoute)
               .Assert(ROUTE_LIST)
               .Assert((token, env) => CheckToken(token,
                                                  TokenType.Operator,
                                                  () => ((RouteSubsetExpression)env.Get<RouteExpression>()).SubsetOperator = token.Value))
               .Assert("Save route", SaveContextRoute)
               .Assert(EXPRESSION_CONDITION)
               .Assert("Pop route", RestoreContextRoute)
               .Assert(OPERATION)
               .Assert("Commit route", CommitContextRoute);
        }

        private void EXPRESSION_CONDITION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("and EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.And)
               .Assert(EXPRESSION_CONDITION_BODY)
               .Assert("Commit AND expression", env => env.Set(new LogicalExpression(LogicalOperator.And, env.Get<RouteExpression[]>())));

            ctx.Sequence("or EXPRESSION_CONDITION_BODY")
               .Assert((t, env) => t.Type == TokenType.Or)
               .Assert(EXPRESSION_CONDITION_BODY)
               .Assert("Commit OR expression", env => env.Set(new LogicalExpression(LogicalOperator.Or, env.Get<RouteExpression[]>())));

            ctx.Sequence("EXPRESSION_CONDITION_BODY")
               .Assert(EXPRESSION_CONDITION_BODY)
               .Assert("Commit AND expression", env => env.Set(new LogicalExpression(LogicalOperator.And, env.Get<RouteExpression[]>())));
        }

        private void EXPRESSION_CONDITION_BODY(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("ArrayStart EXPRESSION_LIST ArrayEnd")
               .Assert((t, env) => t.Type == TokenType.ArrayStart)
               .Assert("Create route list", env => env.Set(new RouteExpression[] { }))
               .Assert(EXPRESSION_LIST)
               .Assert((t, env) => t.Type == TokenType.ArrayEnd);
        }

        private void EXPRESSION_LIST(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("EXPRESSION EXPRESSION_LIST")
               .Assert(EXPRESSION)
               .Assert(EXPRESSION_LIST);

            ctx.Sequence("EMPTY")
               .Assert();
        }

        private void OPERATION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("not operator value")
               .Assert("Create operator", env => env.Set(new OperatorExpression()))
               .Assert((t, env) => CheckToken(t, TokenType.Not, () => env.Get<OperatorExpression>().Inverted = true))
               .Assert((t, env) => CheckToken(t, TokenType.Operator, () => env.Get<OperatorExpression>().Name = t.Value))
               .Assert((t, env) => CheckToken(t, TokenType.Value, () => env.Get<OperatorExpression>().Value = t.Value))
               .Assert("Commit operator", env => env.Get<RouteExpression>().Operator = env.Get<OperatorExpression>());

            ctx.Sequence("operator value")
               .Assert("Create operator", env => env.Set(new OperatorExpression()))
               .Assert((t, env) => CheckToken(t, TokenType.Operator, () => env.Get<OperatorExpression>().Name = t.Value))
               .Assert((t, env) => CheckToken(t, TokenType.Value, () => env.Get<OperatorExpression>().Value = t.Value))
               .Assert("Commit operator", env => env.Get<RouteExpression>().Operator = env.Get<OperatorExpression>());
        }

        private void ROUTE_LIST(ParseContext<JsonToken> ctx)
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