using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Timbal.Utilities;
using Xunit;

namespace Timbal.Tests.AllocatorTests
{
    public class AllocateProportionallyTests
    {
        [Fact]
        public void should_allocate_propotionally()
        {
            // given
            var settings = new AllocatorSettings { Precision = 5 };
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<string, decimal>
            {
                ["Item 1"] = 2,
                ["Item 2"] = 5
            };

            // when
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, settings);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(2)
                .And.HaveElementAt(0, new KeyValuePair<string, decimal>("Item 1", 71.42857m))
                .And.HaveElementAt(1, new KeyValuePair<string, decimal>("Item 2", 178.57143m));

            actual.Remainder.Should().Be(0m);

            var total = actual.Allocations.Sum(k => k.Value) + actual.Remainder;
            total.Should().Be(amountToAllocate);
        }

        [Fact]
        public void should_allow_mixed_sign_allocation_basis_netting_positive()
        {
            // given
            var settings = new AllocatorSettings { Precision = 5 };
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 4,// 1000
                [202] = -3// -750
            };

            // when
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, settings);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(2)
                .And.HaveElementAt(0, new KeyValuePair<int, decimal>(101, 1000m))
                .And.HaveElementAt(1, new KeyValuePair<int, decimal>(202, -750m));

            actual.Remainder.Should().Be(0m);

            var total = actual.Allocations.Sum(k => k.Value) + actual.Remainder;
            total.Should().Be(amountToAllocate);
        }

        [Fact]
        public void should_allow_mixed_sign_allocation_basis_netting_negative()
        {
            // given
            var settings = new AllocatorSettings { Precision = 5 };
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 4,// -1000
                [202] = -5// 1250
            };

            // when
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, settings);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(2)
                .And.HaveElementAt(0, new KeyValuePair<int, decimal>(101, -1000m))
                .And.HaveElementAt(1, new KeyValuePair<int, decimal>(202, 1250m));

            actual.Remainder.Should().Be(0m);

            var total = actual.Allocations.Sum(k => k.Value) + actual.Remainder;
            total.Should().Be(amountToAllocate);
        }

        [Fact]
        public void should_throw_if_allocation_basis_nets_to_zero()
        {
            // given
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 5,
                [202] = -5,
                [303] = 0
            };

            // when
            Action actual = () => allocationBasis.AllocateProportionally(amountToAllocate);

            // then
            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void should_throw_arg_null()
        {
            // given
            var src = default(IEnumerable<KeyValuePair<int, decimal>>);
            const decimal amountToAllocate = 10m;
            var settings = new AllocatorSettings { Precision = 4 };

            // when
            Action actual = () => src.AllocateProportionally(amountToAllocate, settings);

            // then
            actual.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void should_throw_if_no_recipients()
        {
            // given
            var src = new KeyValuePair<int, decimal>[] { };
            const decimal amountToAllocate = 10m;
            var settings = new AllocatorSettings { Precision = 4 };

            // when
            Action actual = () => src.AllocateProportionally(amountToAllocate, settings);

            // then
            actual.Should().Throw<ArgumentException>();
        }
    }
}
