using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timbal.Utilities
{
    public static class Allocator
    {
        public static IDictionary<int, decimal> AllocateAmountEvenly(this int[] recipients, decimal amount, int precision)
        {
            if (recipients == null) throw new ArgumentNullException(nameof(recipients));

            var rv = new Dictionary<int, decimal>();

            if (!recipients.Any()) return rv;

            var allocation = decimal.Round(amount / recipients.Length, precision);
            var remainder = amount - (allocation * recipients.Length);

            for (int i = 0; i < recipients.Length; i++)
            {
                rv[recipients[i]] = allocation;
            }

            rv[recipients[0]] += remainder;

            return rv;
        }
    }
}
