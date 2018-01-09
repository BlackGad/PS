using System;

namespace PS.Expression.Test2.Fluent
{
    public class ExpressionOperatorBuilder
    {
        private readonly Type _appliedTo;
        private readonly Type _producedResult;
        private readonly ExpressionSchemeOperators _expressionSchemeOperators;
        private readonly string _token;
        private string _key;

        #region Constructors

        public ExpressionOperatorBuilder(ExpressionSchemeOperators expressionSchemeOperators, Type appliedTo, Type producedResult, string token)
        {
            if (expressionSchemeOperators == null) throw new ArgumentNullException(nameof(expressionSchemeOperators));
            if (appliedTo == null) throw new ArgumentNullException(nameof(appliedTo));
            if (producedResult == null) throw new ArgumentNullException(nameof(producedResult));
            if (token == null) throw new ArgumentNullException(nameof(token));

            _expressionSchemeOperators = expressionSchemeOperators;
            _appliedTo = appliedTo;
            _producedResult = producedResult;
            _token = token;
        }

        #endregion

        #region Members

        public ExpressionOperatorBuilder Key(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _key = key;
            return this;
        }

        public ExpressionSchemeOperators Register()
        {
            _expressionSchemeOperators.Register(new ExpressionOperator
            {
                Token = _token,
                AppliedTo = _appliedTo,
                //Factory = 
                Key = _key
            });
            return _expressionSchemeOperators;
        }

        #endregion
    }
}