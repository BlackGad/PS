using System;
using System.Collections.Concurrent;

namespace PS.Data
{
    public class StorageSnapshot
    {
        #region Constructors

        internal StorageSnapshot(ConcurrentDictionary<object, DescriptorSnapshot> snapshots)
        {
            if (snapshots == null) throw new ArgumentNullException(nameof(snapshots));
            Snapshots = snapshots;
        }

        #endregion

        #region Properties

        public ConcurrentDictionary<object, DescriptorSnapshot> Snapshots { get; }

        #endregion
    }
}