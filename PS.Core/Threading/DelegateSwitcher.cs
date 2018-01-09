using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using PS.Linq.Expressions.Extensions;

namespace PS.Threading
{
    public class DelegateSwitcher<TDelegate> where TDelegate : class
    {
        private readonly AutoResetEvent _event;

        private readonly ConcurrentDictionary<object, TDelegate> _storage;
        private readonly Func<object, bool> _switchingPredicate;
        private readonly object _switchLocker;
        private readonly TDelegate _transitionDelegate;

        #region Constructors

        public DelegateSwitcher() : this(null)
        {
        }

        public DelegateSwitcher(Func<object, bool> switchingPredicate)
        {
            if (!typeof(Delegate).IsAssignableFrom(typeof(TDelegate))) throw new InvalidCastException("Generic type is not delegate");
            if (typeof(TDelegate) == typeof(Delegate)) throw new InvalidCastException("Delegate struture cannot be recognized");
            _switchingPredicate = switchingPredicate;
            _switchLocker = new object();
            _storage = new ConcurrentDictionary<object, TDelegate>();
            _event = new AutoResetEvent(false);
            _transitionDelegate = ConstructTransition();
        }

        #endregion

        #region Properties

        public TDelegate Active { get; private set; }

        public TDelegate this[object key]
        {
            get
            {
                TDelegate @delegate;
                var result = _storage.TryGetValue(key, out @delegate);
                if (result) return @delegate;
                throw new InvalidOperationException();
            }
        }

        #endregion

        #region Members

        public void Register(object key, TDelegate del)
        {
            _storage.AddOrUpdate(key, del, (o, existing) => del);
        }

        public void RegisterAndSwitch(object key, TDelegate del)
        {
            lock (_switchLocker)
            {
                Register(key, del);
                Switch(key);
            }
        }

        public bool Switch(object key)
        {
            lock (_switchLocker)
            {
                var previousDelegate = Active;
                Active = _transitionDelegate;

                TDelegate newDelegate;
                var switchDelegate = _storage.TryGetValue(key, out newDelegate) && newDelegate != null;
                if (switchDelegate && _switchingPredicate != null) switchDelegate = _switchingPredicate(key);
                Active = switchDelegate ? newDelegate : previousDelegate;

                _event.Set();
                return switchDelegate;
            }
        }

        private TDelegate ConstructTransition()
        {
            var delegateType = typeof(TDelegate);
            var method = delegateType.GetMethod("Invoke");

            var waitOneMethod = typeof(WaitHandle).GetMethod(nameof(WaitHandle.WaitOne), new Type[] { });
            var activeProperty = typeof(DelegateSwitcher<TDelegate>).GetProperty(nameof(Active));

            var parameterExpressions = method.GetParameters()
                                             .Select(p => Expression.Parameter(p.ParameterType))
                                             .ToList();
            var body = new List<Expression>
            {
                Expression.Call(Expression.Constant(_event), waitOneMethod),
                Expression.Invoke(Expression.Property(Expression.Constant(this), activeProperty), parameterExpressions)
            };
            var result = Expression.Lambda<TDelegate>(body.PackExpressions(), parameterExpressions);

            return result.Compile();
        }

        #endregion
    }
}