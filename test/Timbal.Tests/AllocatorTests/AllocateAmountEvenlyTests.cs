using System;
using System.Linq;
using Timbal.Utilities;
using Xunit;

namespace Timbal.Tests.AllocatorTests
{
    public class AllocateAmountEvenlyTests
    {
        [Fact]
        public void should_allocate_evenly()
        {
            // arrange
            var src = new[] { 111, 211, 322 };
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.Equal(3, actual.Count());
            Assert.Equal(amountToAllocate, actual.Sum(k => k.Value));

            Assert.Contains(actual, k => k.Key == 211 && k.Value == 3.3333m);
            Assert.Contains(actual, k => k.Key == 322 && k.Value == 3.3333m);
            Assert.Contains(actual, k => k.Key == 111 && k.Value == 3.3334m);
        }

        [Fact]
        public void should_throw_arg_null()
        {
            // arrange
            int[] src = null;
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            Action actual = () => src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.Throws<ArgumentNullException>(actual);
        }

        [Fact]
        public void should_throw_if_no_recipients()
        {
            // arrange
            var src = new int[0];
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            Action actual = () => src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.Throws<ArgumentException>(actual);
        }

        [Fact]
        public void should_allow_same_key()
        {
            // arrange
            var src = new[] { 111, 111, 111 };
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.Equal(3, actual.Count());
            Assert.Equal(amountToAllocate, actual.Sum(k => k.Value));

            Assert.Contains(actual, k => k.Key == 111 && k.Value == 3.3334m);
            Assert.Contains(actual, k => k.Key == 111 && k.Value == 3.3333m);
            Assert.Contains(actual, k => k.Key == 111 && k.Value == 3.3333m);
        }
    }
}
