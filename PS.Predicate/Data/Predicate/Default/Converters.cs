using System.Linq;
using PS.Extensions;

namespace PS.Data.Predicate.Default
{
    public class Converters : DescriptorStorage<Converters, PredicateBatchConverter>
    {
        #region Static members

        public static PredicateBatchConverter PrimitiveTypes
        {
            get
            {
                return FromCache(() => new PredicateBatchConverter(t => ObjectExtensions.GetPrimitiveTypes().Contains(t),
                                                                   (type, s) => s.ConvertToPrimitive(type)));
            }
        }

        #endregion
    }
}