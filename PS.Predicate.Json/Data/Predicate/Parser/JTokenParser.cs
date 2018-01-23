using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PS.Data.Logic;
using PS.Data.Parser;
using PS.Data.Predicate.Logic;
using PS.Navigation;
using LogicalExpression = PS.Data.Predicate.Logic.LogicalExpression;

namespace PS.Data.Predicate.Parser
{
    /// <summary>
    ///     Grammar:
    ///     S -> EXPRESSION eos     
    ///     EXPRESSION_CONDITION -> and EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION -> or EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION -> EXPRESSION_CONDITION_BODY
    ///     EXPRESSION_CONDITION_BODY -> ArrayStart EXPRESSION_LIST ArrayEnd
    ///     EXPRESSION_LIST -> EXPRESSION EXPRESSION_LIST
    ///     EXPRESSION_LIST -> EMPTY
    ///     EXPRESSION -> object(route) OPERATION
    ///     EXPRESSION -> EXPRESSION_CONDITION
    ///     EXPRESSION -> object(route) operator EXPRESSION_CONDITION
    ///     EXPRESSION -> object(route) operator EXPRESSION_CONDITION OPERATION
    ///     OPERATION -> not operator value
    ///     OPERATION -> operator value
    /// </summary>
    internal class JTokenParser
    {
        #region Static members

        private static void EXPRESSION(ParseContext<JTokenParserToken> ctx)
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

            ctx.Sequence("EXPRESSION_CONDITION")
               .Rule(EXPRESSION_CONDITION).Action((env, rule) => env.Push(rule.Pop<IExpression>()));
        }

        private static void EXPRESSION_CONDITION(ParseContext<JTokenParserToken> ctx)
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

        private static void EXPRESSION_CONDITION_BODY(ParseContext<JTokenParserToken> ctx)
        {
            ctx.Sequence("ArrayStart EXPRESSION_LIST ArrayEnd")
               .Token("[")
               .Rule(EXPRESSION_LIST).Action((env, rule) => env.Push(rule.Pop<IExpression[]>()))
               .Token("]");
        }

        private static void EXPRESSION_LIST(ParseContext<JTokenParserToken> ctx)
        {
            ctx.Sequence("EXPRESSION EXPRESSION_LIST")
               .Rule(EXPRESSION).Action((env, rule) => env.Get<List<IExpression>>().Add(rule.Pop<IExpression>()))
               .Rule(EXPRESSION_LIST).Action((env, rule) => env.Get<List<IExpression>>().AddRange(rule.Pop<IExpression[]>()))
               .Commit(env => env.Push(env.Pop<List<IExpression>>().ToArray()));

            ctx.Sequence("EMPTY")
               .Commit(env => env.Push(Enumerable.Empty<IExpression>().ToArray()));
        }

        private static void OPERATION(ParseContext<JTokenParserToken> ctx)
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

        private readonly JTokenParserToken[] _tokens;

        #region Constructors

        public JTokenParser(JToken jToken)
        {
            if (jToken == null) throw new ArgumentNullException(nameof(jToken));
            var tokenizer = new JsonTokenizer();
            _tokens = tokenizer.Tokenize(jToken);
        }

        #endregion

        #region Members

        public IExpression Parse()
        {
            var table = new TokenTable<JTokenParserToken>()
                .Add("[", new JTokenParserToken(TokenType.ArrayStart))
                .Add("]", new JTokenParserToken(TokenType.ArrayEnd))
                .Add("object", new JTokenParserToken(TokenType.Object))
                .Add("and", new JTokenParserToken(TokenType.And))
                .Add("or", new JTokenParserToken(TokenType.Or))
                .Add("not", new JTokenParserToken(TokenType.Not))
                .Add("operator", new JTokenParserToken(TokenType.Operator))
                .Add("value", new JTokenParserToken(TokenType.Value))
                .Add("eos", new JTokenParserToken(TokenType.EOS));

            var ctx = ParseContext<JTokenParserToken>.Parse(_tokens, table);

            ctx.Sequence("EXPRESSION eos")
               .Rule(EXPRESSION).Action((env, rule) => env.Push(rule.Pop<IExpression>()))
               .Token("eos");

            if (ctx.FailedBranch != null) throw ctx.FailedBranch.Error;
            if (ctx.SuccessBranch != null) return ctx.SuccessBranch.Environment.Get<IExpression>();

            throw new InvalidOperationException();
        }

        #endregion
    }
}