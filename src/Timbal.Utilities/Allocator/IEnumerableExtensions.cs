﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Timbal.Utilities
{
    public static class IEnumerableExtensions
    {
        public static AllocationResult<T> AllocateEvenly<T>(this IEnumerable<T> items, decimal amount, AllocatorSettings settings = default)
        {
            return items.AllocateProportionally(amount, x => 1, settings);
        }

        public static AllocationResult<T> AllocateProportionally<T>(this IEnumerable<T> items, decimal amount, Func<T, decimal> weightSelector, AllocatorSettings settings = default)
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

            var unallocated = amount;
            var allocations = new List<(T, decimal)>();
            foreach (var i in itemsList)
            {
                var allocation = weightSelector(i) / totalWeight * amount;
                allocation = decimal.Round(allocation, settings.Precision, settings.MidpointRounding);
                unallocated -= allocation;
                allocations.Add((i, allocation));
            }

            return new AllocationResult<T>
            {
                Allocations = allocations,
                Remainder = unallocated
            };
        }
    }
}
