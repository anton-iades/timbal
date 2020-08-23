using System;
using System.Collections.Generic;
using System.Linq;

namespace Timbal.Utilities
{
    public static class Allocator
    {
        public static AllocationResult<TKey> AllocateAmountEvenly<TKey>(this IEnumerable<TKey> recipients, decimal amountToAllocate, AllocatorSettings settings = null)
        {
            return recipients
                .Select(r => new KeyValuePair<TKey, decimal>(r, 1m))
                .AllocateProportionally(amountToAllocate, settings);
        }

        public static AllocationResult<TKey> AllocateProportionally<TKey>(this IEnumerable<KeyValuePair<TKey, decimal>> recipientWeights, decimal amountToAllocate, AllocatorSettings settings = null)
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

            settings ??= new AllocatorSettings { };
            var weightMultiplier = amountToAllocate / totalWeight;

            var allocations = rw
                .Select(weight => new KeyValuePair<TKey, decimal>(weight.Key, decimal.Round(weight.Value * weightMultiplier, settings.Precision, settings.MidpointRounding)))
                .ToList();

            var remainder = amountToAllocate - allocations.Sum(k => k.Value);

            return new AllocationResult<TKey>
            {
                Allocations = allocations,
                Remainder = remainder
            };
        }
    }

    public class AllocatorSettings
    {
        public int Precision { get; set; }
        public MidpointRounding MidpointRounding { get; set; }
    }

    public class AllocationResult<TKey>
    {
        public decimal Remainder { get; set; }
        public IReadOnlyList<KeyValuePair<TKey, decimal>> Allocations { get; set; }
    }
}
