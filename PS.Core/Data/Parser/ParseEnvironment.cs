using System;
using System.Collections.Generic;

namespace PS.Data.Parser
{
    public class ParseEnvironment
    {
        #region Static members

        private static string CreateKey<T>(string key)
        {
            return typeof(T).Name + (key ?? string.Empty);
        }

        #endregion

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

        #endregion

        #region Members

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

        public T Get<T>(Func<T> factory = null)
        {
            return Get(null, factory);
        }

        public T Get<T>(string key, Func<T> factory = null)
        {
            key = CreateKey<T>(key);
            if (_environment.ContainsKey(key)) return (T)_environment[key];
            factory = factory ?? Activator.CreateInstance<T>;
            var result = factory();
            _environment.Add(key, result);
            return result;
        }

        public T Pop<T>()
        {
            return Pop<T>(null);
        }

        public T Pop<T>(string key)
        {
            key = CreateKey<T>(key);
            if (!_environment.ContainsKey(key)) return default(T);
            var result = _environment[key];
            _environment.Remove(key);
            return (T)result;
        }

        public void Push<T>(T value)
        {
            Push(null, value);
        }

        public void Push<T>(string key, T value)
        {
            key = CreateKey<T>(key);
            if (_environment.ContainsKey(key)) _environment[key] = value;
            else _environment.Add(key, value);
        }

        #endregion
    }
}