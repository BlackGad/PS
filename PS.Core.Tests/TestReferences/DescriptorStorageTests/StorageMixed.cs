using PS.Data;

namespace PS.Tests.TestReferences.DescriptorStorageTests
{
    public class StorageMixed : DescriptorStorage<StorageMixed>
    {
        #region Static members

        public static CompexType ComplexDescriptor
        {
            get
            {
                return FromCache(() => new CompexType
                {
                    Value = nameof(CompexType.Value),
                    Description = nameof(CompexType.Description)
                });
            }
        }

        public static int IntegerDescriptor
        {
            get { return FromCache(() => 42); }
        }

        public static string StringDescriptor
        {
            get { return FromCache(() => nameof(StringDescriptor)); }
        }

        #endregion
    }
}