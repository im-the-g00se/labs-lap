using Xunit;
using static LinearAlgebra.Tests.VectorTestSupport;

namespace LinearAlgebra.Tests
{
    public class MathVectorArithmeticTests
    {
        [Fact]
        public void SumNumber_FiniteNonzeroScalar_ReturnsIndependentExpectedCoordinates()
        {
            var source = Vector(4, 8);
            double[] expected = [2, 6];
            IMathVector result = source.SumNumber(-2);
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([4, 8], source);
            source[1] = 88;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void SumNumber_ZeroScalar_ReturnsIndependentCopy()
        {
            var source = Vector(1, 2);
            IMathVector result = source.SumNumber(0);
            Equal([1, 2], result);
            Assert.NotSame(source, result);
            result[0] = 8;
            Equal([1, 2], source);
            source[1] = 88;
            Assert.Equal(2, result[1]);
        }

        [Fact]
        public void SumNumber_EmptyVector_ReturnsNewEmptyVector()
        {
            var source = Vector();
            IMathVector result = source.SumNumber(2);
            Assert.NotSame(source, result);
            Equal([], result);
            Equal([], source);
        }

        [Fact]
        public void SumNumber_NaNScalar_ThrowsAndPreservesSource()
        {
            var source = Vector(1, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.SumNumber(double.NaN));
            Equal([1, 2], source);
        }

        [Fact]
        public void SumNumber_Overflow_ThrowsAndPreservesSource()
        {
            var source = Vector(1, double.MaxValue);
            double number = double.MaxValue;
            Assert.Throws<ArithmeticException>(() => source.SumNumber(number));
            Equal([1, double.MaxValue], source);
        }

        [Fact]
        public void MultiplyNumber_FiniteNonzeroScalar_ReturnsIndependentExpectedCoordinates()
        {
            var source = Vector(4, 8);
            double[] expected = [2, 4];
            IMathVector result = source.MultiplyNumber(0.5);
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([4, 8], source);
            source[1] = 88;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void MultiplyNumber_ZeroScalar_ReturnsExpectedCoordinates()
        {
            var vector = Vector(4, -2);
            IMathVector result = vector.MultiplyNumber(0);
            Equal([0, 0], result);
            Assert.NotSame(vector, result);
            result[0] = 9;
            Equal([4, -2], vector);
            vector[1] = 88;
            Assert.Equal(0, result[1]);
        }

        [Fact]
        public void MultiplyNumber_EmptyVector_ReturnsNewEmptyVector()
        {
            var source = Vector();
            IMathVector result = source.MultiplyNumber(2);
            Assert.NotSame(source, result);
            Equal([], result);
            Equal([], source);
        }

        [Fact]
        public void MultiplyNumber_NaNScalar_ThrowsAndPreservesSource()
        {
            var source = Vector(1, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.MultiplyNumber(double.NaN));
            Equal([1, 2], source);
        }

        [Fact]
        public void MultiplyNumber_Overflow_ThrowsAndPreservesSource()
        {
            var source = Vector(1, double.MaxValue);
            double number = 2;
            Assert.Throws<ArithmeticException>(() => source.MultiplyNumber(number));
            Equal([1, double.MaxValue], source);
        }

        [Fact]
        public void DivideNumber_FiniteNonzeroScalar_ReturnsIndependentExpectedCoordinates()
        {
            var source = Vector(4, 8);
            double[] expected = [-2, -4];
            IMathVector result = source.DivideNumber(-2);
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([4, 8], source);
            source[1] = 88;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void DivideNumber_EmptyVector_ReturnsNewEmptyVector()
        {
            var source = Vector();
            IMathVector result = source.DivideNumber(2);
            Assert.NotSame(source, result);
            Equal([], result);
            Equal([], source);
        }

        [Fact]
        public void DivideNumber_NaNScalar_ThrowsAndPreservesSource()
        {
            var source = Vector(1, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => source.DivideNumber(double.NaN));
            Equal([1, 2], source);
        }

        [Fact]
        public void DivideNumber_Overflow_ThrowsAndPreservesSource()
        {
            var source = Vector(1, double.MaxValue);
            double number = 0.5;
            Assert.Throws<ArithmeticException>(() => source.DivideNumber(number));
            Equal([1, double.MaxValue], source);
        }

        [Fact]
        public void DivideNumber_NonemptyVectorNegativeZero_ThrowsAndPreservesSource()
        {
            double zero = BitConverter.Int64BitsToDouble(long.MinValue);
            double[] coordinates = [1, 1];
            var source = new MathVector(coordinates);
            Assert.Throws<DivideByZeroException>(() => source.DivideNumber(zero));
            Equal(coordinates, source);
        }

        [Fact]
        public void DivideNumber_EmptyVectorPositiveZero_ThrowsAndPreservesSource()
        {
            double zero = 0.0;
            double[] coordinates = [];
            var source = new MathVector(coordinates);
            Assert.Throws<DivideByZeroException>(() => source.DivideNumber(zero));
            Equal(coordinates, source);
        }

        [Fact]
        public void Sum_ForeignImplementation_ReturnsIndependentExpectedResult()
        {
            var left = Vector(4, 8);
            IMathVector right = new ForeignVector(2, 4);
            IMathVector result = left.Sum(right);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
            Equal([6, 12], result);
            result[0] = 99;
            Equal([4, 8], left);
            Equal([2, 4], right);
            left[1] = 88;
            right[1] = 77;
            Assert.Equal(12, result[1]);
        }

        [Fact]
        public void Sum_EmptyOperands_ReturnsNewEmptyVector()
        {
            var left = Vector();
            var right = Vector();
            IMathVector result = left.Sum(right);
            Equal([], result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
        }

        [Fact]
        public void Sum_NullRight_ThrowsAndPreservesLeft()
        {
            var left = Vector(1, 2);
            Assert.Throws<ArgumentNullException>(() => left.Sum(null!));
            Equal([1, 2], left);
        }

        [Fact]
        public void Sum_ShorterRightVector_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            double[] coordinates = [1];
            var right = new MathVector(coordinates);
            Assert.Throws<ArgumentException>(() => left.Sum(right));
            Equal([1, 2], left);
            Equal(coordinates, right);
        }

        [Fact]
        public void Sum_NaNForeignCoordinate_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            IMathVector right = new ForeignVector(1, double.NaN);
            Assert.Throws<ArgumentException>(() => left.Sum(right));
            Equal([1, 2], left);
            Assert.Equal(1, right[0]);
            Assert.Equal(double.NaN, right[1]);
        }

        [Fact]
        public void Sum_Overflow_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, double.MaxValue);
            double second = double.MaxValue;
            var right = Vector(1, second);
            Assert.Throws<ArithmeticException>(() => left.Sum(right));
            Equal([1, double.MaxValue], left);
            Equal([1, second], right);
        }

        [Fact]
        public void Multiply_ForeignImplementation_ReturnsIndependentExpectedResult()
        {
            var left = Vector(4, 8);
            IMathVector right = new ForeignVector(2, 4);
            IMathVector result = left.Multiply(right);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
            Equal([8, 32], result);
            result[0] = 99;
            Equal([4, 8], left);
            Equal([2, 4], right);
            left[1] = 88;
            right[1] = 77;
            Assert.Equal(32, result[1]);
        }

        [Fact]
        public void Multiply_EmptyOperands_ReturnsNewEmptyVector()
        {
            var left = Vector();
            var right = Vector();
            IMathVector result = left.Multiply(right);
            Equal([], result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
        }

        [Fact]
        public void Multiply_NullRight_ThrowsAndPreservesLeft()
        {
            var left = Vector(1, 2);
            Assert.Throws<ArgumentNullException>(() => left.Multiply(null!));
            Equal([1, 2], left);
        }

        [Fact]
        public void Multiply_ShorterRightVector_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            double[] coordinates = [1];
            var right = new MathVector(coordinates);
            Assert.Throws<ArgumentException>(() => left.Multiply(right));
            Equal([1, 2], left);
            Equal(coordinates, right);
        }

        [Fact]
        public void Multiply_NaNForeignCoordinate_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            IMathVector right = new ForeignVector(1, double.NaN);
            Assert.Throws<ArgumentException>(() => left.Multiply(right));
            Equal([1, 2], left);
            Assert.Equal(1, right[0]);
            Assert.Equal(double.NaN, right[1]);
        }

        [Fact]
        public void Multiply_Overflow_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, double.MaxValue);
            double second = 2;
            var right = Vector(1, second);
            Assert.Throws<ArithmeticException>(() => left.Multiply(right));
            Equal([1, double.MaxValue], left);
            Equal([1, second], right);
        }

        [Fact]
        public void Divide_ForeignImplementation_ReturnsIndependentExpectedResult()
        {
            var left = Vector(4, 8);
            IMathVector right = new ForeignVector(2, 4);
            IMathVector result = left.Divide(right);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
            Equal([2, 2], result);
            result[0] = 99;
            Equal([4, 8], left);
            Equal([2, 4], right);
            left[1] = 88;
            right[1] = 77;
            Assert.Equal(2, result[1]);
        }

        [Fact]
        public void Divide_EmptyOperands_ReturnsNewEmptyVector()
        {
            var left = Vector();
            var right = Vector();
            IMathVector result = left.Divide(right);
            Equal([], result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
        }

        [Fact]
        public void Divide_NullRight_ThrowsAndPreservesLeft()
        {
            var left = Vector(1, 2);
            Assert.Throws<ArgumentNullException>(() => left.Divide(null!));
            Equal([1, 2], left);
        }

        [Fact]
        public void Divide_ShorterRightVector_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            double[] coordinates = [1];
            var right = new MathVector(coordinates);
            Assert.Throws<ArgumentException>(() => left.Divide(right));
            Equal([1, 2], left);
            Equal(coordinates, right);
        }

        [Fact]
        public void Divide_NaNForeignCoordinate_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, 2);
            IMathVector right = new ForeignVector(1, double.NaN);
            Assert.Throws<ArgumentException>(() => left.Divide(right));
            Equal([1, 2], left);
            Assert.Equal(1, right[0]);
            Assert.Equal(double.NaN, right[1]);
        }

        [Fact]
        public void Divide_Overflow_ThrowsAndPreservesOperands()
        {
            var left = Vector(1, double.MaxValue);
            double second = 0.5;
            var right = Vector(1, second);
            Assert.Throws<ArithmeticException>(() => left.Divide(right));
            Equal([1, double.MaxValue], left);
            Equal([1, second], right);
        }

        [Fact]
        public void Divide_MiddleCoordinateNegativeZero_ThrowsAndPreservesOperands()
        {
            double zero = BitConverter.Int64BitsToDouble(long.MinValue);
            var left = Vector(1, 2, 3);
            double[] coordinates = [1, 1, 1];
            coordinates[1] = zero;
            var right = new MathVector(coordinates);
            Assert.Throws<DivideByZeroException>(() => left.Divide(right));
            Equal([1, 2, 3], left);
            Equal(coordinates, right);
        }

        [Fact]
        public void AddScalarOperator_FiniteScalar_ReturnsIndependentExpectedCoordinates()
        {
            var source = Vector(4, 8);
            double[] expected = [6, 10];
            IMathVector result = source + 2;
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([4, 8], source);
            source[1] = 88;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void AddScalarOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) + 2);
        }

        [Fact]
        public void SubtractScalarOperator_FiniteScalar_ReturnsIndependentExpectedCoordinates()
        {
            var source = Vector(4, 8);
            double[] expected = [2, 6];
            IMathVector result = source - 2;
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([4, 8], source);
            source[1] = 88;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void SubtractScalarOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) - 2);
        }

        [Fact]
        public void MultiplyScalarOperator_FiniteScalar_ReturnsIndependentExpectedCoordinates()
        {
            var source = Vector(4, 8);
            double[] expected = [8, 16];
            IMathVector result = source * 2;
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([4, 8], source);
            source[1] = 88;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void MultiplyScalarOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) * 2);
        }

        [Fact]
        public void DivideScalarOperator_FiniteScalar_ReturnsIndependentExpectedCoordinates()
        {
            var source = Vector(4, 8);
            double[] expected = [2, 4];
            IMathVector result = source / 2;
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([4, 8], source);
            source[1] = 88;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void DivideScalarOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) / 2);
        }

        [Fact]
        public void AddVectorOperator_CompatibleVectors_ReturnsIndependentExpectedCoordinates()
        {
            var left = Vector(4, 8);
            var right = Vector(2, 4);
            double[] expected = [6, 12];
            IMathVector result = left + right;
            Equal(expected, result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
            result[0] = 99;
            Equal([4, 8], left);
            Equal([2, 4], right);
            left[1] = 88;
            right[1] = 77;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void AddVectorOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) + Vector(1));
        }

        [Fact]
        public void SubtractVectorOperator_CompatibleVectors_ReturnsIndependentExpectedCoordinates()
        {
            var left = Vector(4, 8);
            var right = Vector(2, 4);
            double[] expected = [2, 4];
            IMathVector result = left - right;
            Equal(expected, result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
            result[0] = 99;
            Equal([4, 8], left);
            Equal([2, 4], right);
            left[1] = 88;
            right[1] = 77;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void SubtractVectorOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) - Vector(1));
        }

        [Fact]
        public void MultiplyVectorOperator_CompatibleVectors_ReturnsIndependentExpectedCoordinates()
        {
            var left = Vector(4, 8);
            var right = Vector(2, 4);
            double[] expected = [8, 32];
            IMathVector result = left * right;
            Equal(expected, result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
            result[0] = 99;
            Equal([4, 8], left);
            Equal([2, 4], right);
            left[1] = 88;
            right[1] = 77;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void MultiplyVectorOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) * Vector(1));
        }

        [Fact]
        public void DivideVectorOperator_CompatibleVectors_ReturnsIndependentExpectedCoordinates()
        {
            var left = Vector(4, 8);
            var right = Vector(2, 4);
            double[] expected = [2, 2];
            IMathVector result = left / right;
            Equal(expected, result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
            result[0] = 99;
            Equal([4, 8], left);
            Equal([2, 4], right);
            left[1] = 88;
            right[1] = 77;
            Assert.Equal(expected[1], result[1]);
        }

        [Fact]
        public void DivideVectorOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) / Vector(1));
        }

        [Fact]
        public void ScalarProductOperator_CompatibleVectors_ReturnsExpectedScalar()
        {
            var left = Vector(3, 4);
            var right = Vector(1, 2);
            double actual = left % right;
            Assert.Equal(11.0, actual);
            Equal([3, 4], left);
            Equal([1, 2], right);
        }

        [Fact]
        public void ScalarProductOperator_NullLeft_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((MathVector)null!) % Vector(1));
        }
    }
}
