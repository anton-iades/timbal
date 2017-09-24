using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timbal.Utilities;

namespace Timbal.Tests.AllocatorTests
{
    [TestClass]
    public class AllocateProportionallyTests
    {
        [TestMethod]
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
            Assert.AreEqual<int>(2, actual.Count());
            Assert.AreEqual<decimal>(amountToAllocate, actual.Sum(k => k.Value));

            Assert.IsTrue(actual.Any(k => k.Key == 101 && k.Value == 71.42857m));
            Assert.IsTrue(actual.Any(k => k.Key == 202 && k.Value == 178.57143m));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void should_throw_arg_null()
        {
            // arrange
            IEnumerable<KeyValuePair<int, decimal>> src = null;
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateProportionally(amountToAllocate, allocationPrecision).ToList();

            // assert
        }

        [TestMethod]
        public void should_return_empty_collection()
        {
            // arrange
            var src = Enumerable.Empty<KeyValuePair<int, decimal>>();
            const decimal amountToAllocate = 10m;
            const int allocationPrecision = 4;

            // act
            var actual = src.AllocateProportionally(amountToAllocate, allocationPrecision).ToList();

            // assert
            Assert.AreEqual<int>(0, actual.Count());
        }
    }
}
