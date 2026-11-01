using Xunit;
using static LinearAlgebra.Tests.VectorTestSupport;

namespace LinearAlgebra.Tests
{
    public class MathVectorNumericTests
    {
        public static IEnumerable<object[]> DotProductCases()
        {
            foreach (VectorOperation operation in new[] { VectorOperation.ScalarMultiply, VectorOperation.Remainder })
            {
                yield return [operation, Array.Empty<double>(), Array.Empty<double>(), 0.0];
                yield return [operation, new double[] { 0, 0 }, new double[] { 3, 4 }, 0.0];
                yield return [operation, new double[] { 1, 0 }, new double[] { 0, 1 }, 0.0];
                yield return [operation, new double[] { 3, 4 }, new double[] { 3, 4 }, 25.0];
                yield return [operation, new double[] { 3, 4 }, new double[] { 1, 2 }, 11.0];
                yield return [operation, new double[] { -3, -4 }, new double[] { 1, 2 }, -11.0];
                yield return [operation, new double[] { 0.5, 0.25 }, new double[] { 0.5, 0.5 }, 0.375];
                yield return [operation, new double[] { 1e100 }, new double[] { 1e100 }, 1e200];
            }
        }

        [Theory]
        [MemberData(nameof(DotProductCases))]
        public void ScalarProduct_ValidCoordinates_ReturnsExpectedScalar(VectorOperation operation, double[] leftCoordinates, double[] rightCoordinates, double expected)
        {
            var left = new MathVector(leftCoordinates);
            var right = new MathVector(rightCoordinates);
            double result = Assert.IsType<double>(Apply(operation, left, right));
            if (Math.Abs(expected) < 1e100)
            {
                Assert.Equal(expected, result);
            }
            else
            {
                Close(expected, result);
            }
            Equal(leftCoordinates, left);
            Equal(rightCoordinates, right);
        }

        [Theory]
        [InlineData(VectorOperation.ScalarMultiply)]
        [InlineData(VectorOperation.Remainder)]
        public void ScalarProduct_SameInstance_ReturnsSquaredLengthWithoutMutation(VectorOperation operation)
        {
            var vector = Vector(3, 4);
            Assert.Equal(25.0, Assert.IsType<double>(Apply(operation, vector, vector)));
            Equal([3, 4], vector);
        }

        [Theory]
        [InlineData(VectorOperation.ScalarMultiply)]
        [InlineData(VectorOperation.Remainder)]
        public void ScalarProduct_SumOverflows_ThrowsAndPreservesOperands(VectorOperation operation)
        {
            var left = Vector(double.MaxValue, double.MaxValue);
            var right = Vector(1, 1);
            Assert.Throws<ArithmeticException>(() => Apply(operation, left, right));
            Equal([double.MaxValue, double.MaxValue], left);
            Equal([1, 1], right);
        }

        [Theory]
        [InlineData(VectorOperation.ScalarMultiply)]
        [InlineData(VectorOperation.Remainder)]
        public void ScalarProduct_ForeignImplementation_ReturnsExpectedScalar(VectorOperation operation)
        {
            var left = Vector(3, 4);
            IMathVector right = new ForeignVector(1, 2);
            Assert.Equal(11.0, Assert.IsType<double>(Apply(operation, left, right)));
            Equal([3, 4], left);
            Equal([1, 2], right);
        }

        public static IEnumerable<object[]> DistanceCases()
        {
            yield return [Array.Empty<double>(), Array.Empty<double>(), 0.0, true];
            yield return [new double[] { 0, 0 }, new double[] { 0, 0 }, 0.0, true];
            yield return [new double[] { 3, 4 }, new double[] { 3, 4 }, 0.0, true];
            yield return [new double[] { 3, 4 }, new double[] { 1, 2 }, Math.Sqrt(8), false];
            yield return [new double[] { -3, -4 }, new double[] { 0, 0 }, 5.0, true];
            yield return [new double[] { 0.5 }, new double[] { 0.25 }, 0.25, true];
            yield return [new double[] { 3e200, 4e200 }, new double[] { 0, 0 }, 5e200, false];
            yield return [new double[] { 3e-200, 4e-200 }, new double[] { 0, 0 }, 5e-200, false];
            yield return [new double[] { double.Epsilon }, new double[] { 0 }, double.Epsilon, true];
        }

        [Theory]
        [MemberData(nameof(DistanceCases))]
        public void CalcDistance_RepresentableDistance_ReturnsExpectedValue(double[] leftCoordinates, double[] rightCoordinates, double expected, bool exact)
        {
            var left = new MathVector(leftCoordinates);
            var right = new MathVector(rightCoordinates);
            double actual = left.CalcDistance(right);
            if (exact)
            {
                Assert.Equal(expected, actual);
            }
            else
            {
                Close(expected, actual);
            }
            Equal(leftCoordinates, left);
            Equal(rightCoordinates, right);
        }

        [Fact]
        public void CalcDistance_ReversedOperands_ReturnsSameDistance()
        {
            var left = Vector(-3, 1, 0.5);
            var right = Vector(2, -4, 0);
            Assert.Equal(left.CalcDistance(right), right.CalcDistance(left));
        }

        [Fact]
        public void CalcDistance_SameInstance_ReturnsZeroWithoutMutation()
        {
            var vector = Vector(3, 4);
            Assert.Equal(0, vector.CalcDistance(vector));
            Equal([3, 4], vector);
        }

        [Fact]
        public void CalcDistance_ForeignImplementation_ReturnsExpectedDistance()
        {
            var left = Vector(3, 4);
            IMathVector right = new ForeignVector(1, 2);
            Close(Math.Sqrt(8), left.CalcDistance(right));
            Equal([3, 4], left);
            Equal([1, 2], right);
        }

        [Fact]
        public void CalcDistance_NormOverflows_ThrowsAndPreservesOperands()
        {
            var left = Vector(double.MaxValue, double.MaxValue);
            var right = Vector(0, 0);
            Assert.Throws<ArithmeticException>(() => left.CalcDistance(right));
            Equal([double.MaxValue, double.MaxValue], left);
            Equal([0, 0], right);
        }
    }
}
