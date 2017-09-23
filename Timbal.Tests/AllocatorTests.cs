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

            Assert.IsTrue(actual.ContainsKey(1));
            Assert.AreEqual<decimal>(3.3334m, actual[1]);

            Assert.IsTrue(actual.ContainsKey(2));
            Assert.AreEqual<decimal>(3.3333m, actual[2]);

            Assert.IsTrue(actual.ContainsKey(3));
            Assert.AreEqual<decimal>(3.3333m, actual[3]);
        }
    }
}
