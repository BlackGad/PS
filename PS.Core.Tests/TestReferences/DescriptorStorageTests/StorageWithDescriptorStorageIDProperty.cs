using PS.Data;

namespace PS.Tests.TestReferences.DescriptorStorageTests
{
    [DescriptorStorage(IDProperty = nameof(CompexType.ID))]
    public class StorageWithDescriptorStorageIDProperty : DescriptorStorage<StorageWithDescriptorStorageIDProperty>
    {
        #region Static members


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