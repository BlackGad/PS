using PS.Data;

namespace PS.Tests.TestReferences.DescriptorStorageTests
{
    public class StorageWithIDProperty : DescriptorStorage<StorageWithIDProperty>
    {
        #region Static members

        [Descriptor(IDProperty = nameof(CompexType.ID))]
        public static CompexType Alpha
        {
            get
            {
                return FromCache(() => new CompexType
                {
                    ID = nameof(Alpha),
                    Value = nameof(CompexType.Value),
                    Description = nameof(CompexType.Description)
                });
            }
        }

        [Descriptor(IDProperty = nameof(CompexType.ID))]
        public static CompexType Bravo
        {
            get
            {
                return FromCache(() => new CompexType
                {
                    ID = nameof(Bravo),
                    Value = nameof(CompexType.Value),
                    Description = nameof(CompexType.Description)
                });
            }
        }

        [Descriptor(IDProperty = nameof(CompexType.ID))]
        public static CompexType Charlie
        {
            get
            {
                return FromCache(() => new CompexType
                {
                    ID = nameof(Charlie),
                    Value = nameof(CompexType.Value),
                    Description = nameof(CompexType.Description)
                });
            }
        }

        #endregion
    }
}