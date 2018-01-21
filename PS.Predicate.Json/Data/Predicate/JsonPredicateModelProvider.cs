using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PS.Data.Logic;
using PS.Data.Parser;
using PS.Data.Predicate.Logic;
using PS.Extensions;
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
        #region Static members

        //internal static void RestoreContextRoute(ParseEnvironment env)
        //{
        //    var snapshot = (Tuple<RouteExpression[], RouteExpression>)env[env.Id];
        //    ((RouteSubsetExpression)snapshot.Item2).Subset = env.Get<LogicalExpression>();

        //    env.Push(snapshot.Item1);
        //    env.Push(snapshot.Item2);
        //    env[env.Id] = null;
        //}

        //internal static void SaveContextRoute(ParseEnvironment env)
        //{
        //    env[env.Id] = new Tuple<RouteExpression[], RouteExpression>(env.Pop<RouteExpression[]>(), env.Pop<RouteExpression>());
        //}

        private static void EXPRESSION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("object(route) OPERATION")
               .Token("object").Action((env, t) => env.Push(Route.Parse(t.Value)))
               .Rule(OPERATION).Action((env, rule) => env.Push(rule.Pop<OperatorExpression>()))
               .Commit(env => env.Push<IExpression>(new RouteExpression
               {
                   Route = env.Pop<Route>(),
                   Operator = env.Pop<OperatorExpression>()
               }));

            ctx.Sequence("object(route) operator EXPRESSION_CONDITION")
               .Token("object").Action((env, t) => env.Push(Route.Parse(t.Value)))
               .Token("operator").Action((env, t) => env.Push(nameof(RouteSubsetExpression.Query), t.Value))
               .Rule(EXPRESSION_CONDITION).Action((env, rule) => env.Push(nameof(RouteSubsetExpression.Subset), rule.Pop<IExpression>()))
               .Commit(env => env.Push<IExpression>(new RouteSubsetExpression
               {
                   Route = env.Pop<Route>(),
                   Query = env.Pop<string>(nameof(RouteSubsetExpression.Query)),
                   Subset = env.Pop<IExpression>(nameof(RouteSubsetExpression.Subset))
               }));

            ctx.Sequence("object(route) operator EXPRESSION_CONDITION OPERATION")
               .Token("object").Action((env, t) => env.Push(Route.Parse(t.Value)))
               .Token("operator").Action((env, t) => env.Push(nameof(RouteSubsetExpression.Query), t.Value))
               .Rule(EXPRESSION_CONDITION).Action((env, rule) => env.Push(nameof(RouteSubsetExpression.Subset), rule.Pop<IExpression>()))
               .Rule(OPERATION).Action((env, rule) => env.Push(rule.Pop<OperatorExpression>()))
               .Commit(env => env.Push<IExpression>(new RouteSubsetExpression
               {
                   Route = env.Pop<Route>(),
                   Query = env.Pop<string>(nameof(RouteSubsetExpression.Query)),
                   Operator = env.Pop<OperatorExpression>(),
                   Subset = env.Pop<IExpression>(nameof(RouteSubsetExpression.Subset))
               }));
        }

        private static void EXPRESSION_CONDITION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("and EXPRESSION_CONDITION_BODY")
               .Token("and")
               .Rule(EXPRESSION_CONDITION_BODY).Action((env, rule) => env.Get<List<IExpression>>().AddRange(rule.Pop<IExpression[]>()))
               .Commit(env =>
               {
                   var expressions = env.Pop<List<IExpression>>();
                   env.Push<IExpression>(new LogicalExpression(LogicalOperator.And, expressions));
               });

            ctx.Sequence("or EXPRESSION_CONDITION_BODY")
               .Token("or")
               .Rule(EXPRESSION_CONDITION_BODY).Action((env, rule) => env.Get<List<IExpression>>().AddRange(rule.Pop<IExpression[]>()))
               .Commit(env =>
               {
                   var expressions = env.Pop<List<IExpression>>();
                   env.Push<IExpression>(new LogicalExpression(LogicalOperator.Or, expressions));
               });

            ctx.Sequence("EXPRESSION_CONDITION_BODY")
               .Rule(EXPRESSION_CONDITION_BODY).Action((env, rule) => env.Get<List<IExpression>>().AddRange(rule.Pop<IExpression[]>()))
               .Commit(env =>
               {
                   var expressions = env.Pop<List<IExpression>>();
                   env.Push<IExpression>(new LogicalExpression(LogicalOperator.And, expressions));
               });
        }

        private static void EXPRESSION_CONDITION_BODY(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("ArrayStart EXPRESSION_LIST ArrayEnd")
               .Token("[")
               .Rule(EXPRESSION_LIST).Action((env, rule) => env.Push(rule.Pop<IExpression[]>()))
               .Token("]");
        }

        private static void EXPRESSION_LIST(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("EXPRESSION EXPRESSION_LIST")
               .Rule(EXPRESSION).Action((env, rule) => env.Get<List<IExpression>>().Add(rule.Pop<IExpression>()))
               .Rule(EXPRESSION_LIST).Action((env, rule) => env.Get<List<IExpression>>().AddRange(rule.Pop<IExpression[]>()))
               .Commit(env => env.Push(env.Pop<List<IExpression>>().ToArray()));

            ctx.Sequence("EXPRESSION_CONDITION EXPRESSION_LIST")
               .Rule(EXPRESSION_CONDITION).Action((env, rule) => env.Get<List<IExpression>>().Add(rule.Pop<IExpression>()))
               .Rule(EXPRESSION_LIST).Action((env, rule) => env.Get<List<IExpression>>().AddRange(rule.Pop<IExpression[]>()))
               .Commit(env => env.Push(env.Pop<List<IExpression>>().ToArray()));

            ctx.Sequence("EMPTY")
               .Commit(env=> env.Push(Enumerable.Empty<IExpression>().ToArray()));
        }

        private static void OPERATION(ParseContext<JsonToken> ctx)
        {
            ctx.Sequence("not operator value")
               .Token("not")
               .Token("operator").Action((env, t) => env.Push(nameof(OperatorExpression.Name), t.Value))
               .Token("value").Action((env, t) => env.Push(nameof(OperatorExpression.Value), t.Value))
               .Commit(env => env.Push(new OperatorExpression
               {
                   Inverted = true,
                   Value = env.Pop<string>(nameof(OperatorExpression.Value)),
                   Name = env.Pop<string>(nameof(OperatorExpression.Name))
               }));

            ctx.Sequence("operator value")
               .Token("operator").Action((env, t) => env.Push(nameof(OperatorExpression.Name), t.Value))
               .Token("value").Action((env, t) => env.Push(nameof(OperatorExpression.Value), t.Value))
               .Commit(env => env.Push(new OperatorExpression
               {
                   Value = env.Pop<string>(nameof(OperatorExpression.Value)),
                   Name = env.Pop<string>(nameof(OperatorExpression.Name))
               }));
        }

        #endregion

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

        public IExpression Provide()
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
               .Rule(EXPRESSION_CONDITION).Action((env, rule)=> env.Push(rule.Pop<IExpression>()))
               .Token("eos");

            var s = ctx.ToString("d");

            if (ctx.FailedBranch != null) throw ctx.FailedBranch.Error;
            if (ctx.SuccessBranch != null) return ctx.SuccessBranch.Environment.Get<IExpression>();

            throw new InvalidOperationException();
        }

        #endregion
    }
}