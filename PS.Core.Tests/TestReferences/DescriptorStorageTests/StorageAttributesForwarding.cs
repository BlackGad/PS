using System.ComponentModel.DataAnnotations;
using PS.Data;

namespace PS.Tests.TestReferences.DescriptorStorageTests
{
    public class StorageAttributesForwarding : DescriptorStorage<StorageAttributesForwarding>
    {
        #region Static members

        [Descriptor(ID = nameof(StorageAttributesForwarding))]
        [Display(Name = nameof(Alpha))]
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

        #endregion
    }
}