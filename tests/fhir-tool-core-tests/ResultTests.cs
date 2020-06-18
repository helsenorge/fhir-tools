using System;
using Xunit;

namespace FhirTool.Core.Tests
{
    public class ResultTests
    {
        [Theory]
        [InlineData(10, 4, 6)]
        public void CanAddTwoNumbersUsingOkResult(int expected, int a, int b)
        {
            var aOk = new Ok<int>(a);
            var result = aOk + b;
            Assert.Equal(expected, result);
        }
    }
}
