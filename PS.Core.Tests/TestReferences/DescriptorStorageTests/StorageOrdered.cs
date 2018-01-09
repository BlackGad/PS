using PS.Data;

namespace PS.Tests.TestReferences.DescriptorStorageTests
{
    public class StorageOrdered : DescriptorStorage<StorageOrdered>
    {
        #region Static members

        [Descriptor(Order = 1)]
        public static string Alpha
        {
            get { return FromCache(() => nameof(Alpha)); }
        }

        [Descriptor(Order = 2)]
        public static string Bravo
        {
            get { return FromCache(() => nameof(Bravo)); }
        }

        [Descriptor(Order = 3)]
        public static string Charlie
        {
            get { return FromCache(() => nameof(Charlie)); }
        }

        #endregion
    }
}