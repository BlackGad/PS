using System;
using System.Collections.Generic;
using System.Linq;
using PS.Extensions;

namespace PS.Data.Parser
{
    public class ParseEnvironment
    {
        private readonly Dictionary<object, object> _environment;

        #region Constructors

        public ParseEnvironment()
        {
            _environment = new Dictionary<object, object>();
            Id = Guid.NewGuid();
        }

        #endregion

        #region Properties

        public Guid Id { get; }

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

                if (value == null) _environment.Remove(key);
            }
        }

        #endregion

        #region Members

        public void Add<T>(T value)
        {
            var array = this[typeof(T[])].Enumerate<T>();
            this[typeof(T[])] = array.Union(new[] { value }).ToArray();
        }

        public ParseEnvironment Clone()
        {
            return Clone(new ParseEnvironment());
        }

        public ParseEnvironment Clone(ParseEnvironment env)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));
            env._environment.Clear();
            foreach (var pair in _environment)
            {
                env._environment.Add(pair.Key, pair.Value);
            }

            return env;
        }

        public T Peek<T>()
        {
            return (T)this[typeof(T)];
        }

        public T Pop<T>()
        {
            var result = Peek<T>();
            Push(default(T));
            return result;
        }

        public void Push<T>(T value)
        {
            this[typeof(T)] = value;
        }

        #endregion
    }
}