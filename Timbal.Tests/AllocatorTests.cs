using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Timbal.Utilities;

namespace Timbal.Tests
{
    [TestClass]
    public class AllocatorTests
    {
        [TestMethod]
        public void should_allocate_evenly()
        {
            // arrange
            var src = new[] { 111, 211, 322 };
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.AreEqual<int>(3, actual.Count());
            Assert.AreEqual<decimal>(amountToAllocate, actual.Sum(k => k.Value));

            Assert.IsTrue(actual.Any(k => k.Key == 111 && k.Value == 3.3334m));
            Assert.IsTrue(actual.Any(k => k.Key == 211 && k.Value == 3.3333m));
            Assert.IsTrue(actual.Any(k => k.Key == 322 && k.Value == 3.3333m));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void should_throw_arg_null()
        {
            // arrange
            int[] src = null;
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // assert
        }

        [TestMethod]
        public void should_return_empty_collection()
        {
            // arrange
            var src = new int[0];
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.AreEqual<int>(0, actual.Count());
        }

        [TestMethod]
        public void should_allow_same_key()
        {
            // arrange
            var src = new[] { 111, 111, 111 };
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.AreEqual<int>(3, actual.Count());
            Assert.AreEqual<decimal>(amountToAllocate, actual.Sum(k => k.Value));

            Assert.IsTrue(actual.Any(k => k.Key == 111 && k.Value == 3.3334m));
            Assert.IsTrue(actual.Any(k => k.Key == 111 && k.Value == 3.3333m));
            Assert.IsTrue(actual.Any(k => k.Key == 111 && k.Value == 3.3333m));
        }
    }
}
