using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PS.Data;
using PS.Expression.Json.Extensions;
using PS.Expression.Test2;
using PS.Navigation;

namespace PS.Expression.Json
{
    /// <summary>
    /// https://jack-vanlightly.com/blog/2016/2/11/implementing-a-dsl-parser
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

        /// <summary>
        /// </summary>
        /// <param name="json"></param>
        /// <remarks>
        ///     Grammar Rule:
        ///     S -> EXPRESSION_CONDITION
        /// </remarks>
        /// <returns></returns>
        public JsonToken[] Parse(string json)
        {
            var tokenizer = new JsonTokenizer();
            var tokens = tokenizer.Tokenize((JToken)JsonConvert.DeserializeObject(json));
            var sequence = new TokenSequence<JsonToken>(tokens);

            EXPRESSION_CONDITION(new ParseContext(sequence));

            return tokens;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     Grammar Rule:
        ///     EXPRESSION -> object(property) EXPRESSION
        ///     EXPRESSION -> object(not) object(operator) value
        ///     EXPRESSION -> object(operator) value
        /// </remarks>
        /// <returns></returns>
        private void EXPRESSION(ParseContext ctx)
        {
            ctx.Sequence.CheckAheadToken(TokenType.Object);
            var token = ctx.Sequence.Lookahead();

            //property found
            if (true)
            {
                using (ctx.RouteProperty(token.Value))
                {
                    ctx.Sequence.Next();
                    EXPRESSION(ctx);
                    return;
                }
            }

            if (string.Equals(token.Value, "not", StringComparison.InvariantCultureIgnoreCase))
            {
                //not operator detected
                ctx.Sequence.Next();
                token = ctx.Sequence.Lookahead();
            }

            ctx.Sequence.CheckAheadToken(TokenType.Object);

            //Operator not found
            if (false)
            {
                throw new ParserException($"Operator {token.Value} not supported");
            }

            ctx.Sequence.CheckAheadToken(TokenType.Value);
            //value detected
            ctx.Sequence.Next();
        }

        private void EXPRESSION_CONDITION(ParseContext ctx)
        {
            ctx.Sequence.CheckAheadToken(TokenType.Any);

            var token = ctx.Sequence.Lookahead();
            if (token.Type == TokenType.Object && string.Equals(token.Value, "and", StringComparison.InvariantCultureIgnoreCase))
            {
                ctx.Sequence.Next();
                EXPRESSION_CONDITION_BODY(ctx);
            }
            else if (token.Type == TokenType.Object && string.Equals(token.Value, "or", StringComparison.InvariantCultureIgnoreCase))
            {
                ctx.Sequence.Next();
                EXPRESSION_CONDITION_BODY(ctx);
            }
            else
            {
                EXPRESSION_CONDITION_BODY(ctx);
            }

            ctx.Sequence.CheckAheadToken(TokenType.EOS);
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     Grammar Rule:
        ///     EXPRESSION_CONDITION_BODY -> ArrayStart EXPRESSION_LIST ArrayEnd
        /// </remarks>
        /// <returns></returns>
        private void EXPRESSION_CONDITION_BODY(ParseContext ctx)
        {
            ctx.Sequence.CheckAheadToken(TokenType.Any);

            var token = ctx.Sequence.Lookahead();
            if (token.Type == TokenType.ArrayStart)
            {
                ctx.Sequence.Next();
                EXPRESSION_LIST(ctx);
                ctx.Sequence.CheckAheadToken(TokenType.ArrayEnd);
                ctx.Sequence.Next();
            }
            else token.ThrowUnexpectedToken();
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     Grammar Rules:
        ///     EXPRESSION_LIST -> EXPRESSION EXPRESSION_LIST
        ///     EXPRESSION_LIST -> EXPRESSION
        /// </remarks>
        /// <returns></returns>
        private void EXPRESSION_LIST(ParseContext ctx)
        {
            ctx.Sequence.CheckAheadToken(TokenType.Any);
            if (ctx.Sequence.Lookahead().Type == TokenType.ArrayEnd) return;

            EXPRESSION(ctx);
        }

        #endregion

        #region Nested type: ParseContext

        class ParseContext
        {
            private Route _currentPropertyRoute;

            #region Constructors

            public ParseContext(TokenSequence<JsonToken> sequence)
            {
                Sequence = sequence;
                _currentPropertyRoute = Route.Create();
            }

            #endregion

            #region Properties

            public TokenSequence<JsonToken> Sequence { get; }

            #endregion

            #region Members

            public IDisposable RouteProperty(string name)
            {
                var previous = _currentPropertyRoute;
                _currentPropertyRoute = Route.Create(_currentPropertyRoute, name);
                return new DisposableDelegate(() => _currentPropertyRoute = previous);
            }

            #endregion
        }

        #endregion
    }
}