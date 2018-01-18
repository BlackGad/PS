using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PS.Extensions;

namespace PS.Data
{
    /// <summary>
    ///     Helpful class for static descriptors initialization.
    /// </summary>
    /// <typeparam name="TStorage">Current storage type.</typeparam>
    public abstract class DescriptorStorage<TStorage> : DescriptorStorage<TStorage, object> where TStorage : DescriptorStorage<TStorage, object>
    {
    }

    /// <summary>
    ///     Helpful class for static descriptors initialization.
    /// </summary>
    /// <typeparam name="TStorage">Current storage type.</typeparam>
    /// <typeparam name="TArgument">Scope descriptors with type</typeparam>
    public abstract class DescriptorStorage<TStorage, TArgument> where TStorage : DescriptorStorage<TStorage, TArgument>
    {
        #region Constants

        // ReSharper disable once StaticMemberInGenericType
        private static readonly ConcurrentDictionary<object, object> CachedValues = new ConcurrentDictionary<object, object>();

        #endregion

        #region Static members

        public static bool Contains(object id)
        {
            return DescriptorStorage.Contains(typeof(TStorage), id);
        }

        public static TArgument Get(object id)
        {
            TArgument descriptor;
            return TryGet(id, out descriptor) ? descriptor : default(TArgument);
        }

        public static bool TryGet(object id, out TArgument descriptor)
        {
            object descriptorInstance;
            var result = DescriptorStorage.TryGet(typeof(TStorage), id, out descriptorInstance);
            descriptor = (TArgument)descriptorInstance;
            return result;
        }

        protected static T FromCache<T>(Expression<Func<T>> factory)
        {
            if (factory == null) return default(T);
            var debugViewProperty = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic);
            var debugView = debugViewProperty?.GetValue(factory) as string;
            if (debugView == null) debugView = factory.ToString();
            return (T)CachedValues.GetOrAdd(debugView.GetHashCode(), o => factory.Compile().Invoke());
        }

        public static IEnumerable<TArgument> All => DescriptorStorage.GetAll(typeof(TStorage)).OfType<TArgument>();

        #endregion
    }

    public static class DescriptorStorage
    {
        #region Constants

        private static readonly ConcurrentDictionary<Type, Lazy<StorageSnapshot>> Cache =
            new ConcurrentDictionary<Type, Lazy<StorageSnapshot>>();

        #endregion

        #region Static members

        public static bool Contains(Type type, object id)
        {
            return GetDescriptorSnapshot(type, id) != null;
        }

        public static IEnumerable<object> GetAll(Type type)
        {
            var info = GetStorageSnapshot(type);
            if (info == null)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                throw new InvalidCastException($"{type.FullName} is not inherited from DescriptorStorage");
            }
            return info.Snapshots
                       .Values
                       .OrderBy(f => f.Attributes.OfType<DescriptorAttribute>().FirstOrDefault()?.Order ?? 0)
                       .Select(v => v.Getter());
        }

        public static DescriptorSnapshot GetDescriptorSnapshot(Type type, object id)
        {
            DescriptorSnapshot snapshot = null;
            GetStorageSnapshot(type)?.Snapshots.TryGetValue(id, out snapshot);
            return snapshot;
        }

        public static StorageSnapshot GetStorageSnapshot(Type type)
        {
            if (type == null) return null;
            Func<StorageSnapshot> valueFactory = () =>
            {
                var types = type.EnumerateHierarchy().ToList();
                var storageType = types.LastOrDefault(t => t.IsGenericType &&
                                                           (t.GetGenericTypeDefinition() == typeof(DescriptorStorage<>) ||
                                                            t.GetGenericTypeDefinition() == typeof(DescriptorStorage<,>)));

                if (storageType == null) return null;
                types = types.TakeWhile(t => t != storageType).ToList();

                var snapshots = types.SelectMany(t => t.GetProperties(BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public)
                                                       .Where(p => p.CanRead)
                                                       .Where(p =>
                                                       {
                                                           var displayAttribute = p.GetCustomAttribute<DescriptorAttribute>();
                                                           return displayAttribute == null || !displayAttribute.Ignore;
                                                       }))
                                     .Select(p => new DescriptorSnapshot(() => p.GetValue(null), p.GetCustomAttributes().ToArray()));

                return new StorageSnapshot(snapshots.ToConcurrentDictionary(factory => factory.GetID(type), true));
            };
            return Cache.GetOrAdd(type, key => new Lazy<StorageSnapshot>(valueFactory, true)).Value;
        }

        public static bool TryGet(Type type, object id, out object descriptor)
        {
            descriptor = null;

            var descriptorSnapshot = GetDescriptorSnapshot(type, id);
            if (descriptorSnapshot == null) return false;
            descriptor = descriptorSnapshot.Getter();
            return true;
        }

        #endregion
    }
}