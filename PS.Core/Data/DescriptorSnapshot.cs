using System;
using System.Linq;
using System.Reflection;
using PS.Extensions;

namespace PS.Data
{
    public class DescriptorSnapshot
    {
        private object _id;

        #region Constructors

        public DescriptorSnapshot(Func<object> getter, Attribute[] attributes)
        {
            if (getter == null) throw new ArgumentNullException(nameof(getter));
            Getter = getter;
            Attributes = attributes.Enumerate().ToArray();
        }

        #endregion

        #region Properties

        public Attribute[] Attributes { get; }

        public Func<object> Getter { get; }

        #endregion

        #region Members

        public object GetID(Type storageType)
        {
            if (_id != null) return _id;

            var instance = Getter();
            if (instance == null) return null;

            var descriptorAttribute = Attributes.OfType<DescriptorAttribute>().FirstOrDefault();
            var descriptorStorageAttribute = storageType.GetCustomAttribute<DescriptorStorageAttribute>();

            if (descriptorAttribute?.ID != null) _id = descriptorAttribute.ID;
            if (_id == null)
            {
                var idProperty = descriptorAttribute?.IDProperty ?? descriptorStorageAttribute?.IDProperty;
                if (idProperty != null)
                {
                    _id = instance.GetType()
                                  .GetProperty(idProperty,
                                               BindingFlags.Public |
                                               BindingFlags.NonPublic |
                                               BindingFlags.Instance)?
                                  .GetValue(instance);
                }
            }

            return _id ?? (_id = instance.GetHashCode());
        }

        #endregion
    }
}