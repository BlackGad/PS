using System;

namespace PS.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DescriptorAttribute : Attribute
    {
        #region Properties

        public object ID { get; set; }
        public string IDProperty { get; set; }
        public bool Ignore { get; set; }
        public int Order { get; set; }

        #endregion
    }
}