extern alias R3;
extern alias R4;

using System.Linq;
using System.Collections.Generic;
using R3Model = R3::Hl7.Fhir.Model;
using R4Model = R4::Hl7.Fhir.Model;

namespace FhirTool.Core.FhirWrappers
{
    public static class BundleTypeWrapperExtensions
    {
        private static Dictionary<BundleTypeWrapper, R3Model.Bundle.BundleType> R3Map =
            new Dictionary<BundleTypeWrapper, R3Model.Bundle.BundleType>
            {
                { BundleTypeWrapper.Batch, R3Model.Bundle.BundleType.Batch },
                { BundleTypeWrapper.Collection, R3Model.Bundle.BundleType.Collection },
                { BundleTypeWrapper.Transaction, R3Model.Bundle.BundleType.Transaction},
            };
        private static Dictionary<BundleTypeWrapper, R4Model.Bundle.BundleType> R4Map =
            new Dictionary<BundleTypeWrapper, R4Model.Bundle.BundleType>
            {
                { BundleTypeWrapper.Batch, R4Model.Bundle.BundleType.Batch },
                { BundleTypeWrapper.Collection, R4Model.Bundle.BundleType.Collection },
                { BundleTypeWrapper.Transaction, R4Model.Bundle.BundleType.Transaction },
            };

        public static R3Model.Bundle.BundleType ToR3(this BundleTypeWrapper me)
        {
            return R3Map[me];
        }

        public static R4Model.Bundle.BundleType ToR4(this BundleTypeWrapper me)
        {
            return R4Map[me];
        }

        public static BundleTypeWrapper Wrap(this R3Model.Bundle.BundleType? me)
        {
            return me.HasValue ? R3Map.ToDictionary(keySelector => keySelector.Value, valueSelector => valueSelector.Key)[me.Value] : default ;
        }

        public static BundleTypeWrapper Wrap(this R4Model.Bundle.BundleType? me)
        {
            return me.HasValue ? R4Map.ToDictionary(keySelector => keySelector.Value, valueSelector => valueSelector.Key)[me.Value] : default;
        }
    }

    public enum BundleTypeWrapper
    {
        Transaction = 2,
        Batch = 4,
        Collection = 8,
    }
}