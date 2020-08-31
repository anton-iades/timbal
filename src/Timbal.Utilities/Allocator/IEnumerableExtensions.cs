using System;
using System.Collections.Generic;
using System.Linq;

namespace Timbal.Utilities
{
    public static class IEnumerableExtensions
    {
        public static AllocationResult<T> AllocateEvenly<T>(this IEnumerable<T> items, decimal amount, AllocatorSettings settings = null)
        {
            return items
                .AllocateProportionally(amount, x => 1, settings);
        }

        public static AllocationResult<T> AllocateProportionally<T>(this IEnumerable<T> items, decimal amount, Func<T, decimal> weightSelector, AllocatorSettings settings = null)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (weightSelector is null)
            {
                throw new ArgumentNullException(nameof(weightSelector));
            }

            var itemsList = items.ToList();

            if (!itemsList.Any())
            {
                throw new ArgumentException(Errors.NO_RECIPIENTS);
            }

            var totalWeight = itemsList.Sum(weightSelector);

            if (totalWeight == decimal.Zero)
            {
                throw new ArgumentException(Errors.ALLOCATION_BASIS_ZERO);
            }

            settings ??= new AllocatorSettings { };
            var weightMultiplier = amount / totalWeight;

            var allocations = itemsList
                .Select(i => (Item: i, Allocation: decimal.Round(weightSelector(i) * weightMultiplier, settings.Precision, settings.MidpointRounding)))
                .ToList();

            var remainder = amount - allocations.Sum(k => k.Allocation);

            return new AllocationResult<T>
            {
                Allocations = allocations,
                Remainder = remainder
            };
        }
    }
}
