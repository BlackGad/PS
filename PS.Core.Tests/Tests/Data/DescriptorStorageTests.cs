using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;
using PS.Data;
using PS.Tests.TestReferences.DescriptorStorageTests;

namespace PS.Tests.Data
{
    [TestFixture]
    public class DescriptorStorageTests
    {
        [Test]
        public void All_ByID_DuplicateFailure()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => StorageWithDuplicateID.All.ToArray());
        }

        [Test]
        public void All_Mixed_Success()
        {
            var orderedEnumeration = StorageMixed.All.ToArray();
            var expected = new object[]
            {
                StorageMixed.StringDescriptor,
                StorageMixed.IntegerDescriptor,
                StorageMixed.ComplexDescriptor
            };

            CollectionAssert.AreEquivalent(expected, orderedEnumeration);
        }

        [Test]
        public void All_Ordering_Success()
        {
            var orderedEnumeration = StorageOrdered.All.ToArray();
            var expected = new[]
            {
                StorageOrdered.Alpha,
                StorageOrdered.Bravo,
                StorageOrdered.Charlie
            };

            CollectionAssert.AreEqual(expected, orderedEnumeration);
        }

        [Test]
        public void Cached_Descriptor_Success()
        {
            var firstEntry = StorageMixed.ComplexDescriptor;
            var secondEntry = StorageMixed.ComplexDescriptor;
            Assert.IsTrue(ReferenceEquals(firstEntry, secondEntry));
        }

        [Test]
        public void Contains_ByID_Success()
        {
            Assert.IsTrue(StorageWithID.Contains(nameof(StorageWithID.Alpha)));
            Assert.IsTrue(StorageWithID.Contains(nameof(StorageWithID.Bravo)));
            Assert.IsTrue(StorageWithID.Contains(nameof(StorageWithID.Charlie)));
            Assert.IsFalse(StorageWithID.Contains(nameof(StorageWithID)));
        }

        [Test]
        public void Get_ByDescriptorStorageIDProperty_Success()
        {
            Assert.AreEqual(StorageWithDescriptorStorageIDProperty.Alpha,
                            StorageWithDescriptorStorageIDProperty.Get(nameof(StorageWithDescriptorStorageIDProperty.Alpha)));
            Assert.AreEqual(StorageWithDescriptorStorageIDProperty.Bravo,
                            StorageWithDescriptorStorageIDProperty.Get(nameof(StorageWithDescriptorStorageIDProperty.Bravo)));
            Assert.AreEqual(StorageWithDescriptorStorageIDProperty.Charlie,
                            StorageWithDescriptorStorageIDProperty.Get(nameof(StorageWithDescriptorStorageIDProperty.Charlie)));
        }

        [Test]
        public void Get_ByID_Success()
        {
            Assert.AreEqual(StorageWithID.Alpha, StorageWithID.Get(nameof(StorageWithID.Alpha)));
            Assert.AreEqual(StorageWithID.Bravo, StorageWithID.Get(nameof(StorageWithID.Bravo)));
            Assert.AreEqual(StorageWithID.Charlie, StorageWithID.Get(nameof(StorageWithID.Charlie)));
        }

        [Test]
        public void Get_ByIDProperty_Success()
        {
            Assert.AreEqual(StorageWithIDProperty.Alpha, StorageWithIDProperty.Get(nameof(StorageWithID.Alpha)));
            Assert.AreEqual(StorageWithIDProperty.Bravo, StorageWithIDProperty.Get(nameof(StorageWithID.Bravo)));
            Assert.AreEqual(StorageWithIDProperty.Charlie, StorageWithIDProperty.Get(nameof(StorageWithID.Charlie)));
        }

        [Test]
        public void GetAll_InvalidType_Failure()
        {
            Assert.Throws<InvalidCastException>(() => DescriptorStorage.GetAll(typeof(object)));
            Assert.Throws<ArgumentNullException>(() => DescriptorStorage.GetAll(null));
        }

        [Test]
        public void Snapshot_AttributesForwarding_Success()
        {
            var storageSnapshot = DescriptorStorage.GetStorageSnapshot(typeof(StorageAttributesForwarding));
            Assert.NotNull(storageSnapshot);

            Assert.IsTrue(storageSnapshot.Snapshots.Any());
            var descriptorSnapshot = storageSnapshot.Snapshots.First().Value;
            Assert.NotNull(descriptorSnapshot?.Attributes);

            var descriptorAttribute = descriptorSnapshot.Attributes.OfType<DescriptorAttribute>().FirstOrDefault();
            Assert.NotNull(descriptorAttribute);
            Assert.AreEqual(nameof(StorageAttributesForwarding), descriptorAttribute.ID);

            var displayAttribute = descriptorSnapshot.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
            Assert.NotNull(displayAttribute);
            Assert.AreEqual(nameof(StorageAttributesForwarding.Alpha), displayAttribute.GetName());
        }

        [Test]
        public void TryGet_InvalidData_Success()
        {
            object descriptor;
            Assert.IsFalse(DescriptorStorage.TryGet(typeof(object), nameof(StorageWithID.Alpha), out descriptor));
            Assert.IsNull(descriptor);
            Assert.IsFalse(DescriptorStorage.TryGet(typeof(StorageWithID), nameof(StorageWithID), out descriptor));
            Assert.IsNull(descriptor);
        }
    }
}