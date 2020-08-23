using System;
using System.Collections.Generic;
using System.Linq;
using Timbal.Utilities;
using Xunit;

namespace Timbal.Tests.AllocatorTests
{
    public class AllocateProportionallyTests
    {
        [Fact]
        public void should_allocate_propotionally()
        {
            // arrange
            const int precision = 5;
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 2,
                [202] = 5
            };

            // act
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, precision).ToList();

            // assert
            Assert.Equal(2, actual.Count());
            Assert.Equal(amountToAllocate, actual.Sum(k => k.Value));

            Assert.Contains(actual, k => k.Key == 101 && k.Value == 71.42857m);
            Assert.Contains(actual, k => k.Key == 202 && k.Value == 178.57143m);
        }

        [Fact]
        public void should_allow_mixed_sign_allocation_basis_netting_positive()
        {
            // arrange
            const int precision = 5;
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 4,// 1000
                [202] = -3// -750
            };

            // act
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, precision).ToList();

            // assert
            Assert.Equal(2, actual.Count());
            Assert.Equal(amountToAllocate, actual.Sum(k => k.Value));

            Assert.Contains(actual, k => k.Key == 101 && k.Value == 1000m);
            Assert.Contains(actual, k => k.Key == 202 && k.Value == -750m);
        }

        [Fact]
        public void should_allow_mixed_sign_allocation_basis_netting_negative()
        {
            // arrange
            const int precision = 5;
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 4,// -1000
                [202] = -5// 1250
            };

            // act
            var actual = allocationBasis.AllocateProportionally(amountToAllocate, precision).ToList();

            // assert
            Assert.Equal(2, actual.Count());
            Assert.Equal(amountToAllocate, actual.Sum(k => k.Value));

            Assert.Contains(actual, k => k.Key == 101 && k.Value == -1000m);
            Assert.Contains(actual, k => k.Key == 202 && k.Value == 1250m);
        }

        [Fact]
        public void should_throw_if_allocation_basis_nets_to_zero()
        {
            // arrange
            const int precision = 5;
            const decimal amountToAllocate = 250m;
            var allocationBasis = new Dictionary<int, decimal>
            {
                [101] = 5,
                [202] = -5,
                [303] = 0
            };

            // act
            Action actual = () => allocationBasis.AllocateProportionally(amountToAllocate, precision).ToList();

            // assert
            Assert.Throws<ArgumentException>(actual);
        }

        [Fact]
        public void should_throw_arg_null()
        {
            // arrange
            IEnumerable<KeyValuePair<int, decimal>> src = null;
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            Action actual = () => src.AllocateProportionally(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.Throws<ArgumentNullException>(actual);
        }

        [Fact]
        public void should_throw_if_no_recipients()
        {
            // arrange
            var src = Enumerable.Empty<KeyValuePair<int, decimal>>();
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            Action actual = () => src.AllocateProportionally(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.Throws<ArgumentException>(actual);
        }
    }
}
