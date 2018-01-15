using System;

namespace PS.Query.Logic
{
    public class FactorizeParams
    {
        private Func<LogicalOperator, ILogicalExpression> _compositionFactory;

        #region Constructors

        public FactorizeParams(Func<object, bool, object> convertFactory)
        {
            if (convertFactory == null) throw new ArgumentNullException(nameof(convertFactory));
            ConvertFactory = convertFactory;
        }

        #endregion

        #region Properties

        public Func<LogicalOperator, ILogicalExpression> CompositionFactory
        {
            get { return _compositionFactory ?? (_compositionFactory = op => new LogicalExpression(op)); }
            set { _compositionFactory = value; }
        }

        public Func<object, bool, object> ConvertFactory { get; }

        #endregion
    }
}