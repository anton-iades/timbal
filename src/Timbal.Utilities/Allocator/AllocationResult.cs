using System.Collections.Generic;

namespace Timbal.Utilities
{
    public class AllocationResult<TKey>
    {
        public decimal Remainder { get; set; }
        public IReadOnlyList<(TKey Item, decimal Value)> Allocations { get; set; }
    }
}
