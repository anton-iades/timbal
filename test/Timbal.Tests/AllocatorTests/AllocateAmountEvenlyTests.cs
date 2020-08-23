using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Timbal.Utilities;
using Xunit;

namespace Timbal.Tests.AllocatorTests
{
    public class AllocateAmountEvenlyTests
    {
        [Fact]
        public void should_allocate_evenly_positive()
        {
            // given
            var src = new[] { 111, 211, 322 };
            const decimal amountToAllocate = 20m;

            // when
            var actual = src.AllocateAmountEvenly(amountToAllocate);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(3)
                .And.HaveElementAt(0, new KeyValuePair<int, decimal>(111, 7m))
                .And.HaveElementAt(1, new KeyValuePair<int, decimal>(211, 7m))
                .And.HaveElementAt(2, new KeyValuePair<int, decimal>(322, 7m));

            actual.Remainder.Should().Be(-1m);

            var total = actual.Allocations.Sum(k => k.Value) + actual.Remainder;
            total.Should().Be(amountToAllocate);
        }

        [Fact]
        public void should_allocate_evenly_negative()
        {
            // given
            var src = new[] { 111, 211, 322 };
            const decimal amountToAllocate = -20m;

            // when
            var actual = src.AllocateAmountEvenly(amountToAllocate);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(3)
                .And.HaveElementAt(0, new KeyValuePair<int, decimal>(111, -7m))
                .And.HaveElementAt(1, new KeyValuePair<int, decimal>(211, -7m))
                .And.HaveElementAt(2, new KeyValuePair<int, decimal>(322, -7m));

            actual.Remainder.Should().Be(1m);

            var total = actual.Allocations.Sum(k => k.Value) + actual.Remainder;
            total.Should().Be(amountToAllocate);
        }

        [Fact]
        public void should_throw_arg_null()
        {
            // given
            var src = default(int[]);
            const decimal amountToAllocate = 10m;

            // when
            Action actual = () => src.AllocateAmountEvenly(amountToAllocate);

            // then
            actual.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void should_throw_if_no_recipients()
        {
            // given
            var src = new int[] { };
            const decimal amountToAllocate = 10m;

            // when
            Action actual = () => src.AllocateAmountEvenly(amountToAllocate);

            // then
            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void should_allow_same_key()
        {
            // given
            var src = new[] { 111, 111, 111 };
            const decimal amountToAllocate = 10m;
            var settings = new AllocatorSettings { Precision = 4 };

            // when
            var actual = src.AllocateAmountEvenly(amountToAllocate, settings);

            // then
            actual.Should().NotBeNull();

            actual.Allocations.Should().HaveCount(3)
                .And.HaveElementAt(0, new KeyValuePair<int, decimal>(111, 3.3333m))
                .And.HaveElementAt(1, new KeyValuePair<int, decimal>(111, 3.3333m))
                .And.HaveElementAt(2, new KeyValuePair<int, decimal>(111, 3.3333m));

            actual.Remainder.Should().Be(0.0001m);

            var total = actual.Allocations.Sum(k => k.Value) + actual.Remainder;
            total.Should().Be(amountToAllocate);
        }
    }
}
