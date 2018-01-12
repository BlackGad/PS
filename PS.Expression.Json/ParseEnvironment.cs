using System.Collections.Generic;

namespace PS.Expression.Json
{
    public class ParseEnvironment
    {
        private readonly Dictionary<object, object> _environment;

        #region Constructors

        public ParseEnvironment()
        {
            _environment = new Dictionary<object, object>();
        }

        #endregion

        #region Properties

        public object this[object key]
        {
            get
            {
                if (_environment.ContainsKey(key)) return _environment[key];
                return null;
            }
            set
            {
                if (_environment.ContainsKey(key)) _environment[key] = value;
                else _environment.Add(key, value);
            }
        }

        #endregion

        #region Members

        public ParseEnvironment Clone()
        {
            var result = new ParseEnvironment();
            foreach (var pair in _environment)
            {
                result._environment.Add(pair.Key, pair.Value);
            }

            return result;
        }

        #endregion
    }
}