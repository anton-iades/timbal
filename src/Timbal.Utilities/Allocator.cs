using System;
using System.Collections.Generic;
using System.Linq;

namespace Timbal.Utilities
{
    public static class Allocator
    {
        public static AllocationResult<T> AllocateAmountEvenly<T>(this IEnumerable<T> recipients, decimal amountToAllocate, AllocatorSettings settings = null)
        {
            return recipients
                .AllocateProportionally(amountToAllocate, x => 1, settings);
        }

        public static AllocationResult<T> AllocateProportionally<T>(this IEnumerable<T> recipients, decimal amountToAllocate, Func<T, decimal> weightSelector, AllocatorSettings settings = null)
        {
            if (recipients is null)
            {
                throw new ArgumentNullException(nameof(recipients));
            }

            var recipientsList = recipients.ToList();

            if (!recipientsList.Any())
            {
                throw new ArgumentException(Errors.NO_RECIPIENTS);
            }

            var totalWeight = recipientsList.Sum(weightSelector);

            if (totalWeight == decimal.Zero)
            {
                throw new ArgumentException(Errors.ALLOCATION_BASIS_ZERO);
            }

            settings ??= new AllocatorSettings { };
            var weightMultiplier = amountToAllocate / totalWeight;

            var allocations = recipientsList
                .Select(r => (Recipient: r, Allocation: decimal.Round(weightSelector(r) * weightMultiplier, settings.Precision, settings.MidpointRounding)))
                .ToList();

            var remainder = amountToAllocate - allocations.Sum(k => k.Allocation);

            return new AllocationResult<T>
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
        public IReadOnlyList<(TKey Recipient, decimal Allocation)> Allocations { get; set; }
    }
}
