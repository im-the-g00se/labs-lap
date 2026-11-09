using Xunit;
using static LinearAlgebra.Tests.VectorTestSupport;

namespace LinearAlgebra.Tests
{
    public class MathVectorNumericTests
    {
        [Fact]
        public void ScalarMultiply_CompatibleVectors_ReturnsExpectedScalar()
        {
            var left = Vector(-0.5, -0.25);
            IMathVector right = new ForeignVector(0.5, 0.5);
            double actual = left.ScalarMultiply(right);
            Assert.Equal(-0.375, actual);
            Equal([-0.5, -0.25], left);
            Equal([0.5, 0.5], right);
        }

        [Fact]
        public void ScalarMultiply_EmptyVectors_ReturnsExpectedScalar()
        {
            var left = Vector();
            var right = Vector();
            double actual = left.ScalarMultiply(right);
            Assert.Equal(0.0, actual);
            Equal([], left);
            Equal([], right);
        }

        [Fact]
        public void ScalarMultiply_ZeroVector_ReturnsExpectedScalar()
        {
            var left = Vector(0, 0);
            var right = Vector(3, 4);
            double actual = left.ScalarMultiply(right);
            Assert.Equal(0.0, actual);
            Equal([0, 0], left);
            Equal([3, 4], right);
        }

        [Fact]
        public void ScalarMultiply_OrthogonalVectors_ReturnsExpectedScalar()
        {
            var left = Vector(1, 0);
            var right = Vector(0, 1);
            double actual = left.ScalarMultiply(right);
            Assert.Equal(0.0, actual);
            Equal([1, 0], left);
            Equal([0, 1], right);
        }

        [Fact]
        public void ScalarMultiply_NullRight_ThrowsAndPreservesLeft()
        {
            var left = Vector(1, 2);
            Assert.Throws<ArgumentNullException>(() => left.ScalarMultiply(null!));
            Equal([1, 2], left);
        }

        [Fact]
        public void ScalarMultiply_ShorterRightVector_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            double[] coordinates = [1];
            var right = new MathVector(coordinates);
            Assert.Throws<ArgumentException>(() => left.ScalarMultiply(right));
            Equal([1, 2], left);
            Equal(coordinates, right);
        }

        [Fact]
        public void ScalarMultiply_NaNForeignCoordinate_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            IMathVector right = new ForeignVector(1, double.NaN);
            Assert.Throws<ArgumentException>(() => left.ScalarMultiply(right));
            Equal([1, 2], left);
            Assert.Equal(1, right[0]);
            Assert.Equal(double.NaN, right[1]);
        }

        [Fact]
        public void ScalarMultiply_ProductOverflows_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, double.MaxValue);
            double second = 2;
            var right = Vector(1, second);
            Assert.Throws<ArithmeticException>(() => left.ScalarMultiply(right));
            Equal([1, double.MaxValue], left);
            Equal([1, second], right);
        }

        [Fact]
        public void ScalarMultiply_SumOverflows_ThrowsAndPreservesOperands()
        {
            var left = Vector(double.MaxValue, double.MaxValue);
            var right = Vector(1, 1);
            Assert.Throws<ArithmeticException>(() => left.ScalarMultiply(right));
            Equal([double.MaxValue, double.MaxValue], left);
            Equal([1, 1], right);
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
        public void CalcDistance_EmptyVectors_ReturnsExpectedValue()
        {
            var left = Vector();
            var right = Vector();
            double actual = left.CalcDistance(right);
            Assert.Equal(0.0, actual);
            Equal([], left);
            Equal([], right);
        }

        [Fact]
        public void CalcDistance_EqualVectors_ReturnsExpectedValue()
        {
            var left = Vector(3, 4);
            var right = Vector(3, 4);
            double actual = left.CalcDistance(right);
            Assert.Equal(0.0, actual);
            Equal([3, 4], left);
            Equal([3, 4], right);
        }

        [Fact]
        public void CalcDistance_NullRight_ThrowsAndPreservesLeft()
        {
            var left = Vector(1, 2);
            Assert.Throws<ArgumentNullException>(() => left.CalcDistance(null!));
            Equal([1, 2], left);
        }

        [Fact]
        public void CalcDistance_ShorterRightVector_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            double[] coordinates = [1];
            var right = new MathVector(coordinates);
            Assert.Throws<ArgumentException>(() => left.CalcDistance(right));
            Equal([1, 2], left);
            Equal(coordinates, right);
        }

        [Fact]
        public void CalcDistance_NaNForeignCoordinate_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            IMathVector right = new ForeignVector(1, double.NaN);
            Assert.Throws<ArgumentException>(() => left.CalcDistance(right));
            Equal([1, 2], left);
            Assert.Equal(1, right[0]);
            Assert.Equal(double.NaN, right[1]);
        }

        [Fact]
        public void CalcDistance_LargeCoordinates_ReturnsExpectedValue()
        {
            var left = Vector(3e200, 4e200);
            var right = Vector(0, 0);
            double actual = left.CalcDistance(right);
            Close(5e200, actual);
            Equal([3e200, 4e200], left);
            Equal([0, 0], right);
        }

        [Fact]
        public void CalcDistance_SmallCoordinates_ReturnsExpectedValue()
        {
            var left = Vector(3e-200, 4e-200);
            var right = Vector(0, 0);
            double actual = left.CalcDistance(right);
            Close(5e-200, actual);
            Equal([3e-200, 4e-200], left);
            Equal([0, 0], right);
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
