using System;
using System.Collections.Generic;
using System.Linq;

namespace Timbal.Utilities
{
    public static class Allocator
    {
        public static IEnumerable<KeyValuePair<int, decimal>> AllocateAmountEvenly(this IEnumerable<int> recipients, decimal amount, int precision)
        {
            if (recipients == null) throw new ArgumentNullException(nameof(recipients));
            var recipientsArray = recipients.ToArray();

            if (recipientsArray.Length == 0) yield break;

            var generalAllocation = decimal.Round(amount / recipientsArray.Length, precision);
            var remainder = amount % generalAllocation;

            for (int i = 0; i < recipientsArray.Length; i++)
            {
                var allocation = i == 0 ? generalAllocation + remainder : generalAllocation;

                yield return new KeyValuePair<int, decimal>(recipientsArray[i], allocation);
            }
        }
    }
}
