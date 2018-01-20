using System;
using Newtonsoft.Json.Linq;
using PS.Data.Logic;
using PS.Data.Parser;
using PS.Data.Predicate.Logic;
using PS.Navigation;
using LogicalExpression = PS.Data.Predicate.Logic.LogicalExpression;

namespace PS.Data.Predicate
{
    /// <summary>
    ///     Grammar:
    ///     S -> EXPRESSION_CONDITION eos
    ///     EXPRESSION_CONDITION -> and EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION -> or EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION -> EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION_BODY -> ArrayStart EXPRESSION_LIST ArrayEnd
    ///     EXPRESSION_LIST -> EXPRESSION_CONDITION EXPRESSION_LIST
    ///     EXPRESSION_LIST -> EXPRESSION EXPRESSION_LIST
    ///     EXPRESSION_LIST -> EMPTY
    ///     EXPRESSION -> object(route) OPERATION
    ///     EXPRESSION -> object(route) operator EXPRESSION_CONDITION
    ///     EXPRESSION -> object(route) operator EXPRESSION_CONDITION OPERATION
    ///     OPERATION -> not operator value
    ///     OPERATION -> operator value
    /// </summary>
    public class JsonPredicateModelProvider
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

        #region Members

        public LogicalExpression Provide()
        {
            var table = new TokenTable<JsonToken>()
                .Add("[", new JsonToken(TokenType.ArrayStart))
                .Add("]", new JsonToken(TokenType.ArrayEnd))
                .Add("object", new JsonToken(TokenType.Object))
                .Add("and", new JsonToken(TokenType.And))
                .Add("or", new JsonToken(TokenType.Or))
                .Add("not", new JsonToken(TokenType.Not))
                .Add("operator", new JsonToken(TokenType.Operator))
                .Add("value", new JsonToken(TokenType.Value))
                .Add("eos", new JsonToken(TokenType.EOS));

            var ctx = ParseContext<JsonToken>.Parse(_tokens, table);

            ctx.Sequence("EXPRESSION_CONDITION eos")
               .Rule(EXPRESSION_CONDITION)
               .Token("eos");

            var s = ctx.ToString("d");

            if (ctx.FailedBranch != null) throw ctx.FailedBranch.Error;
            if (ctx.SuccessBranch != null) return ctx.SuccessBranch.Environment.Peek<LogicalExpression>();

            throw new InvalidOperationException();
        }

        internal bool CheckNotOperator(JsonToken t, ParseEnvironment env)
        {
            return CheckToken(t, TokenType.Not, () => env.Peek<OperatorExpression>().Inverted = true);
        }

        internal bool CheckToken(JsonToken t, TokenType type, Action action)
        {
            if (t.Type != type) return false;
            action();
            return true;
        }

        internal void CommitContextRoute(ParseEnvironment env)
        {
            env.Add(env.Pop<RouteExpression>());
        }

        internal void RestoreContextRoute(ParseEnvironment env)
        {
            var snapshot = (Tuple<RouteExpression[], RouteExpression>)env[env.Id];
            ((RouteSubsetExpression)snapshot.Item2).Subset = env.Peek<LogicalExpression>();

            env.Push(snapshot.Item1);
            env.Push(snapshot.Item2);
            env[env.Id] = null;
        }

        internal void SaveContextRoute(ParseEnvironment env)
        {
            env[env.Id] = new Tuple<RouteExpression[], RouteExpression>(env.Pop<RouteExpression[]>(), env.Pop<RouteExpression>());
        }

        private void EXPRESSION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("object(route) OPERATION")
               .Action("Create expression", env => env.Push(new RouteExpression()))
               .Token("object").Action("Extract route", env => { env.Peek<RouteExpression>().Route = Route.Parse(env.Peek<JsonToken>().Value); })
               .Rule(OPERATION)
               .Action("Commit route", CommitContextRoute);

            ctx.Sequence("object(route) operator EXPRESSION_CONDITION")
               .Action("Create subset expression", env => env.Push<RouteExpression>(new RouteSubsetExpression()))
               .Token("object").Action("Extract route", env => { env.Peek<RouteExpression>().Route = Route.Parse(env.Peek<JsonToken>().Value); })
               .Token("operator")
               .Action("Extract query operator", env => { ((RouteSubsetExpression)env.Peek<RouteExpression>()).Query = env.Peek<JsonToken>().Value; })
               .Action("Push subset route", SaveContextRoute)
               .Rule(EXPRESSION_CONDITION)
               .Action("Pop subset route", RestoreContextRoute)
               .Action("Commit route", CommitContextRoute);

            ctx.Sequence("object(route) operator EXPRESSION_CONDITION OPERATION")
               .Action("Create subset expression", env => env.Push<RouteExpression>(new RouteSubsetExpression()))
               .Token("object").Action("Extract route", env => { env.Peek<RouteExpression>().Route = Route.Parse(env.Peek<JsonToken>().Value); })
               .Token("operator")
               .Action("Extract query operator", env => { ((RouteSubsetExpression)env.Peek<RouteExpression>()).Query = env.Peek<JsonToken>().Value; })
               .Action("Save subset route", SaveContextRoute)
               .Rule(EXPRESSION_CONDITION)
               .Action("Pop subset route", RestoreContextRoute)
               .Rule(OPERATION)
               .Action("Commit route", CommitContextRoute);
        }

        private void EXPRESSION_CONDITION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("and EXPRESSION_CONDITION_BODY")
               .Token("and")
               .Rule(EXPRESSION_CONDITION_BODY)
               .Action("Commit AND expression", env => env.Push(new LogicalExpression(LogicalOperator.And, env.Peek<RouteExpression[]>())));

            ctx.Sequence("or EXPRESSION_CONDITION_BODY")
               .Token("or")
               .Rule(EXPRESSION_CONDITION_BODY)
               .Action("Commit OR expression", env => env.Push(new LogicalExpression(LogicalOperator.Or, env.Peek<RouteExpression[]>())));

            ctx.Sequence("EXPRESSION_CONDITION_BODY")
               .Rule(EXPRESSION_CONDITION_BODY)
               .Action("Commit AND expression", env => env.Push(new LogicalExpression(LogicalOperator.And, env.Peek<RouteExpression[]>())));
        }

        private void EXPRESSION_CONDITION_BODY(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("ArrayStart EXPRESSION_LIST ArrayEnd")
               .Token("[")
               .Action("Create expression list", env => env.Push(new RouteExpression[] { }))
               .Rule(EXPRESSION_LIST)
               .Token("]");
        }

        private void EXPRESSION_LIST(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("EXPRESSION EXPRESSION_LIST")
               .Rule(EXPRESSION)
               .Rule(EXPRESSION_LIST);

            ctx.Sequence("EXPRESSION_CONDITION EXPRESSION_LIST")
               .Rule(EXPRESSION_CONDITION)
               .Rule(EXPRESSION_LIST);

            ctx.Sequence("EMPTY")
               .Action();
        }

        private void OPERATION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("not operator value")
               .Action("Create operator", env => env.Push(new OperatorExpression()))
               .Token("not").Action(env => { env.Peek<OperatorExpression>().Inverted = true; })
               .Token("operator")
               .Action("Extract operator", env => { env.Peek<OperatorExpression>().Name = env.Peek<JsonToken>().Value; })
               .Token("value").Action("Extract value", env => { env.Peek<OperatorExpression>().Value = env.Peek<JsonToken>().Value; })
               .Action("Commit operator", env => env.Peek<RouteExpression>().Operator = env.Peek<OperatorExpression>());

            ctx.Sequence("operator value")
               .Action("Create operator", env => env.Push(new OperatorExpression()))
               .Token("operator")
               .Action("Extract operator", env => { env.Peek<OperatorExpression>().Name = env.Peek<JsonToken>().Value; })
               .Token("value").Action("Extract value", env => { env.Peek<OperatorExpression>().Value = env.Peek<JsonToken>().Value; })
               .Action("Commit operator", env => env.Peek<RouteExpression>().Operator = env.Peek<OperatorExpression>());
        }

        #endregion
    }
}