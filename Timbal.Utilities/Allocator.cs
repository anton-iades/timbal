using System;
using System.Collections.Generic;
using System.Linq;

namespace Timbal.Utilities
{
    public static class Allocator
    {
        public static IEnumerable<KeyValuePair<int, decimal>> AllocateAmountEvenly(this IEnumerable<int> recipients, decimal amount, int precision)
        {
            return recipients
                .Select(r => new KeyValuePair<int, decimal>(r, 1m))
                .AllocateProportionally(amount, precision);
        }

        public static IEnumerable<KeyValuePair<int, decimal>> AllocateProportionally(this IEnumerable<KeyValuePair<int, decimal>> recipientAllocationBasis, decimal amount, int precision)
        {
            if (recipientAllocationBasis == null) throw new ArgumentNullException(nameof(recipientAllocationBasis));
            var allocationBasisArray = recipientAllocationBasis.ToArray();

            if (allocationBasisArray.Length == 0) throw new ArgumentException(Errors.NO_RECIPIENTS);

            var total = allocationBasisArray.Sum(k => k.Value);

            if (total == 0m) throw new ArgumentException(Errors.ALLOCATION_BASIS_ZERO);

            var rv = allocationBasisArray.Select(k => new KeyValuePair<int, decimal>(k.Key, decimal.Round(k.Value / total * amount, precision))).ToArray();
            var remainder = amount - rv.Sum(k => k.Value);

            for (int i = 0; i < rv.Length; i++)
            {
                var key = rv[i].Key;
                var val = i == 0 ? rv[i].Value + remainder : rv[i].Value;
                yield return new KeyValuePair<int, decimal>(key, val);
            }
        }
    }
}
