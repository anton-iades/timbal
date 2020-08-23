using System;
using System.Collections.Generic;
using System.Linq;

namespace Timbal.Utilities
{
    public static class Allocator
    {
        public static IEnumerable<KeyValuePair<TKey, decimal>> AllocateAmountEvenly<TKey>(this IEnumerable<TKey> recipients, decimal amountToAllocate, int precision)
        {
            return recipients
                .Select(r => new KeyValuePair<TKey, decimal>(r, 1m))
                .AllocateProportionally(amountToAllocate, precision);
        }

        public static IEnumerable<KeyValuePair<TKey, decimal>> AllocateProportionally<TKey>(this IEnumerable<KeyValuePair<TKey, decimal>> recipientWeights, decimal amountToAllocate, int precision)
        {
            if (recipientWeights is null)
            {
                throw new ArgumentNullException(nameof(recipientWeights));
            }

            var rw = recipientWeights.ToList();

            if (!rw.Any())
            {
                throw new ArgumentException(Errors.NO_RECIPIENTS);
            }

            var totalWeight = rw.Sum(k => k.Value);

            if (totalWeight == decimal.Zero)
            {
                throw new ArgumentException(Errors.ALLOCATION_BASIS_ZERO);
            }

            var weightMultiplier = amountToAllocate / totalWeight;

            var rv = rw
                .Select(weight => new KeyValuePair<TKey, decimal>(weight.Key, decimal.Round(weight.Value * weightMultiplier, precision)))
                .ToList();

            var remainder = amountToAllocate - rv.Sum(k => k.Value);
            if (remainder != decimal.Zero)
            {
                rv[0] = new KeyValuePair<TKey, decimal>(rv[0].Key, rv[0].Value + remainder);
            }

            return rv;
        }
    }
}
