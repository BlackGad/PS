using System;

namespace PS.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DescriptorStorageAttribute : Attribute
    {
        #region Properties

        public string IDProperty { get; set; }

        #endregion
    }
}