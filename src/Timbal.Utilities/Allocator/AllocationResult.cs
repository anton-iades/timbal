using System.Collections.Generic;

namespace Timbal.Utilities
{
    public class AllocationResult<TKey>
    {
        public decimal Remainder { get; set; }
        public IReadOnlyList<(TKey Recipient, decimal Allocation)> Allocations { get; set; }
    }
}
