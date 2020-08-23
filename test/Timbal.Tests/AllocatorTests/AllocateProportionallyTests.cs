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
            const int precision = 5;
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 2,
                [202] = 5
            };

            // when
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, precision).ToList();

            // then
            actual.Should().HaveCount(2);
            actual.Sum(k => k.Value).Should().Be(amountToAllocate);

            actual.Should().Contain(k => k.Key == 101 && k.Value == 71.42857m);
            actual.Should().Contain(k => k.Key == 202 && k.Value == 178.57143m);
        }

        [Fact]
        public void should_allow_mixed_sign_allocation_basis_netting_positive()
        {
            // given
            const int precision = 5;
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 4,// 1000
                [202] = -3// -750
            };

            // when
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, precision).ToList();

            // then
            actual.Should().HaveCount(2);
            actual.Sum(k => k.Value).Should().Be(amountToAllocate);

            actual.Should().Contain(k => k.Key == 101 && k.Value == 1000);
            actual.Should().Contain(k => k.Key == 202 && k.Value == -750);
        }

        [Fact]
        public void should_allow_mixed_sign_allocation_basis_netting_negative()
        {
            // given
            const int precision = 5;
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 4,// -1000
                [202] = -5// 1250
            };

            // when
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, precision).ToList();

            // then
            actual.Should().HaveCount(2);
            actual.Sum(k => k.Value).Should().Be(amountToAllocate);

            actual.Should().Contain(k => k.Key == 101 && k.Value == -1000);
            actual.Should().Contain(k => k.Key == 202 && k.Value == 1250);
        }

        [Fact]
        public void should_throw_if_allocation_basis_nets_to_zero()
        {
            // given
            const int precision = 5;
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 5,
                [202] = -5,
                [303] = 0
            };

            // when
            Action actual = () => allocationBasis.AllocateProportionally(amountToAllocate, precision).ToList();

            // then
            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void should_throw_arg_null()
        {
            // given
            IEnumerable<KeyValuePair<int, decimal>> src = null;
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // when
            Action actual = () => src.AllocateProportionally(amountToAllocate, allocationPrecision).ToList();

            // then
            actual.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void should_throw_if_no_recipients()
        {
            // given
            var src = Enumerable.Empty<KeyValuePair<int, decimal>>();
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // when
            Action actual = () => src.AllocateProportionally(amountToAllocate, allocationPrecision).ToList();

            // then
            actual.Should().Throw<ArgumentException>();
        }
    }
}
