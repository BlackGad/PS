using PS.Data;

namespace PS.Tests.TestReferences.DescriptorStorageTests
{
    public class StorageWithDuplicateID : DescriptorStorage<StorageWithDuplicateID>
    {
        #region Static members

        [Descriptor(ID = nameof(StorageWithDuplicateID))]
        public static CompexType Alpha
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

        [Descriptor(ID = nameof(StorageWithDuplicateID))]
        public static CompexType Bravo
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

        #endregion
    }
}