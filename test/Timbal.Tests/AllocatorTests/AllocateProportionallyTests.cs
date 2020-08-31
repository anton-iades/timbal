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
            var elements = new[]
            {
                new {Key= "Item 1", Value = 2},
                new {Key= "Item 2", Value = 5}
            };

            // when
            var actual = elements.AllocateProportionally(amountToAllocate, i => i.Value, settings);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(2)
                .And.HaveElementAt(0, (elements[0], 71.42857m))
                .And.HaveElementAt(1, (elements[1], 178.57143m));

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
            var elements = new[]
            {
                new {Id= 101, Weight = 4},
                new {Id= 202, Weight = -3}
            };

            // when
            var actual = elements.AllocateProportionally(amountToAllocate, i => i.Weight, settings);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(2)
                    .And.HaveElementAt(0, (elements[0], 1000m))
                    .And.HaveElementAt(1, (elements[1], -750m));

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
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, i => i.Value, settings);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(2)
                .And.HaveElementAt(0, (allocationBasis.ElementAt(0), -1000m))
                .And.HaveElementAt(1, (allocationBasis.ElementAt(1), 1250m));

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
            Action actual = () => allocationBasis.AllocateProportionally(amountToAllocate, i => i.Value);

            // then
            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void null_items_is_error()
        {
            // given
            var src = default(IEnumerable<KeyValuePair<int, decimal>>);
            const decimal amountToAllocate = 10m;
            var settings = new AllocatorSettings { Precision = 4 };

            // when
            Action actual = () => src.AllocateProportionally(amountToAllocate, i => i.Value, settings);

            // then
            actual.Should().Throw<ArgumentNullException>().And
                .ParamName.Should().Be("items");
        }

        [Fact]
        public void null_weightSelector_is_error()
        {
            // given
            var items = new[] { "Item1", "Item2", "Item3" };

            // when
            Action actual = () => items.AllocateProportionally(250, null);

            // then
            actual.Should().Throw<ArgumentNullException>().And
                .ParamName.Should().Be("weightSelector");
        }

        [Fact]
        public void should_throw_if_no_recipients()
        {
            // given
            var src = new KeyValuePair<int, decimal>[] { };
            const decimal amountToAllocate = 10m;
            var settings = new AllocatorSettings { Precision = 4 };

            // when
            Action actual = () => src.AllocateProportionally(amountToAllocate, i => i.Value, settings);

            // then
            actual.Should().Throw<ArgumentException>();
        }
    }
}
