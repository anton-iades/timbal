using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
            var src = new[] { 111, 211, 322, };
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision);

            // assert
            Assert.AreEqual<int>(3, actual.Count);
            Assert.AreEqual<decimal>(amountToAllocate, actual.Values.Sum());

            Assert.IsTrue(actual.ContainsKey(111));
            Assert.AreEqual<decimal>(3.3334m, actual[111]);

            Assert.IsTrue(actual.ContainsKey(211));
            Assert.AreEqual<decimal>(3.3333m, actual[211]);

            Assert.IsTrue(actual.ContainsKey(322));
            Assert.AreEqual<decimal>(3.3333m, actual[322]);
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
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision);

            // assert
        }

        [TestMethod]
        public void should_return_empty_dictionary()
        {
            // arrange
            var src = new int[0];
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateAmountEvenly(amountToAllocate, allocationPrecision);

            // assert
            Assert.AreEqual<int>(0, actual.Count);
        }
    }
}
