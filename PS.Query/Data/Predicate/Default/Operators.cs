using System;
using PS.Data;

namespace PS.Query.Data.Predicate.Default
{
    public class Operators : DescriptorStorage<Operators>
    {
        #region Static members

        public static EqualOperatorsGroup<Boolean> Boolean
        {
            get { return FromCache(() => new EqualOperatorsGroup<Boolean>()); }
        }

        public static NumericOperatorsGroup<Byte> Byte
        {
            get { return FromCache(() => new NumericOperatorsGroup<Byte>()); }
        }

        public static NumericOperatorsGroup<Char> Char
        {
            get { return FromCache(() => new NumericOperatorsGroup<Char>()); }
        }

        public static NumericOperatorsGroup<DateTime> DateTime
        {
            get { return FromCache(() => new NumericOperatorsGroup<DateTime>()); }
        }

        public static NumericOperatorsGroup<DateTimeOffset> DateTimeOffset
        {
            get { return FromCache(() => new NumericOperatorsGroup<DateTimeOffset>()); }
        }

        public static NumericOperatorsGroup<Decimal> Decimal
        {
            get { return FromCache(() => new NumericOperatorsGroup<Decimal>()); }
        }

        public static NumericOperatorsGroup<Double> Double
        {
            get { return FromCache(() => new NumericOperatorsGroup<Double>()); }
        }

        public static EqualOperatorsGroup<Guid> Guid
        {
            get { return FromCache(() => new EqualOperatorsGroup<Guid>()); }
        }

        public static NumericOperatorsGroup<Int16> Int16
        {
            get { return FromCache(() => new NumericOperatorsGroup<Int16>()); }
        }

        public static NumericOperatorsGroup<Int32> Int32
        {
            get { return FromCache(() => new NumericOperatorsGroup<Int32>()); }
        }

        public static NumericOperatorsGroup<Int64> Int64
        {
            get { return FromCache(() => new NumericOperatorsGroup<Int64>()); }
        }

        public static NumericOperatorsGroup<SByte> SByte
        {
            get { return FromCache(() => new NumericOperatorsGroup<SByte>()); }
        }

        public static NumericOperatorsGroup<Single> Single
        {
            get { return FromCache(() => new NumericOperatorsGroup<Single>()); }
        }

        public static StringOperatorsGroup String
        {
            get { return FromCache(() => new StringOperatorsGroup()); }
        }

        public static SubsetOperatorsGroup Subset
        {
            get { return FromCache(() => new SubsetOperatorsGroup()); }
        }

        public static NumericOperatorsGroup<UInt16> UInt16
        {
            get { return FromCache(() => new NumericOperatorsGroup<UInt16>()); }
        }

        public static NumericOperatorsGroup<UInt32> UInt32
        {
            get { return FromCache(() => new NumericOperatorsGroup<UInt32>()); }
        }

        public static NumericOperatorsGroup<UInt64> UInt64
        {
            get { return FromCache(() => new NumericOperatorsGroup<UInt64>()); }
        }

        #endregion
    }
}