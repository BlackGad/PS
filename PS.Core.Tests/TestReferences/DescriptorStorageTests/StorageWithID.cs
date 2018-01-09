using PS.Data;

namespace PS.Tests.TestReferences.DescriptorStorageTests
{
    public class StorageWithID : DescriptorStorage<StorageWithID>
    {
        #region Static members

        [Descriptor(ID = nameof(Alpha))]
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

        [Descriptor(ID = nameof(Bravo))]
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

        [Descriptor(ID = nameof(Charlie))]
        public static CompexType Charlie
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