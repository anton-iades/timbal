using System;
using System.Linq;
using FluentAssertions;
using Timbal.Utilities;
using Xunit;

namespace Timbal.Tests.AllocatorTests
{
    public class AllocateAmountEvenlyTests
    {
        [Fact]
        public void should_allocate_evenly()
        {
            // given
            var src = new[] { 111, 211, 322 };
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // when
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // then
            actual.Should().HaveCount(3);
            actual.Sum(k => k.Value).Should().Be(amountToAllocate);

            actual.Should().Contain(k => k.Key == 211 && k.Value == 3.3333m);
            actual.Should().Contain(k => k.Key == 322 && k.Value == 3.3333m);
            actual.Should().Contain(k => k.Key == 111 && k.Value == 3.3334m);
        }

        [Fact]
        public void should_throw_arg_null()
        {
            // given
            var src = default(int[]);
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // when
            Action actual = () => src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // then
            actual.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void should_throw_if_no_recipients()
        {
            // given
            var src = new int[] { };
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // when
            Action actual = () => src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // then
            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void should_allow_same_key() // TODO: no it shouldn't
        {
            // given
            var src = new[] { 111, 111, 111 };
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // when
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // then
            actual.Should().HaveCount(3);
            actual.Sum(k => k.Value).Should().Be(amountToAllocate);

            actual.Should().Contain(k => k.Key == 111 && k.Value == 3.3333m);
            actual.Should().Contain(k => k.Key == 111 && k.Value == 3.3333m);
            actual.Should().Contain(k => k.Key == 111 && k.Value == 3.3334m);
        }
    }
}
