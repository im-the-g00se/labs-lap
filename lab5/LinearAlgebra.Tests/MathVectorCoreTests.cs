using System.Collections;
using Xunit;
using static LinearAlgebra.Tests.VectorTestSupport;

namespace LinearAlgebra.Tests
{
    public class MathVectorCoreTests
    {
        [Fact]
        public void Constructor_Array_CopiesCoordinates()
        {
            double[] source = [1, 2];
            var vector = new MathVector(source);
            source[0] = 99;
            vector[1] = 88;
            Equal([1, 88], vector);
            Assert.Equal([99, 2], source);
        }

        [Fact]
        public void Constructor_List_CopiesCoordinatesAndFixesDimensions()
        {
            var source = new List<double> { 1, 2 };
            var vector = new MathVector(source);
            source[0] = 99;
            source.Add(3);
            vector[1] = 88;
            Equal([1, 88], vector);
            Assert.Equal(2, vector.Dimensions);
            Assert.Equal([99, 2, 3], source);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Constructor_VectorSource_CopiesIndependently(bool interfaceSource)
        {
            IMathVector source = Vector(1, 2);
            var copy = interfaceSource ? new MathVector(source) : new MathVector((MathVector)source);
            source[0] = 99;
            copy[1] = 88;
            Equal([1, 88], copy);
            Equal([99, 2], source);
        }

        [Fact]
        public void Constructor_ForeignInterface_CopiesCoordinates()
        {
            IMathVector source = new ForeignVector(1, 2);
            var copy = new MathVector(source);
            source[0] = 99;
            copy[1] = 88;
            Equal([1, 88], copy);
            Equal([99, 2], source);
        }

        [Fact]
        public void Constructor_SinglePassSource_EnumeratesOnce()
        {
            var source = new SinglePassSource();
            var vector = new MathVector(source);
            Equal([1, 2], vector);
            Assert.Equal(1, source.EnumerationCount);
        }

        [Fact]
        public void Constructor_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MathVector(null!));
        }

        [Theory]
        [InlineData(1)]
        [InlineData("1")]
        [InlineData(null)]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void Constructor_InvalidElement_ThrowsArgumentException(object? element)
        {
            Assert.Throws<ArgumentException>(() => new MathVector(new object?[] { 1.0, element }));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void Dimensions_SourceSize_ReturnsCoordinateCount(int count)
        {
            Assert.Equal(count, new MathVector(new double[count]).Dimensions);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void Indexer_BoundaryIndex_ReadsAndWritesOnlySelectedCoordinate(int index)
        {
            var vector = Vector(1, 2, 3);
            Assert.Equal(index + 1, vector[index]);
            vector[index] = 8;
            double[] expected = [1, 2, 3];
            expected[index] = 8;
            Equal(expected, vector);
            Assert.Equal(3, vector.Dimensions);
        }

        [Fact]
        public void Indexer_SingleCoordinate_ReadsAndWrites()
        {
            var vector = Vector(1);
            Assert.Equal(1, vector[0]);
            vector[0] = 2;
            Equal([2], vector);
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        [InlineData(3, -1)]
        [InlineData(3, 3)]
        public void Indexer_InvalidIndex_ReadThrowsArgumentOutOfRangeException(int count, int index)
        {
            var vector = new MathVector(new double[count]);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector[index]);
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(1, 1)]
        [InlineData(3, -1)]
        [InlineData(3, 3)]
        public void Indexer_InvalidIndex_WriteThrowsAndPreservesCoordinates(int count, int index)
        {
            var vector = new MathVector(new double[count]);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector[index] = 8);
            Equal(new double[count], vector);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void Indexer_NonFiniteValue_ThrowsAndPreservesCoordinates(double value)
        {
            var vector = Vector(1, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector[0] = value);
            Equal([1, 2], vector);
        }

        [Theory]
        [InlineData(2.0)]
        [InlineData(-2.0)]
        [InlineData(0.25)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MaxValue)]
        public void Coordinates_FiniteValue_PreservesExactValue(double value)
        {
            var vector = Vector(value);
            Assert.Equal(BitConverter.DoubleToInt64Bits(value), BitConverter.DoubleToInt64Bits(vector[0]));
            vector[0] = 1;
            vector[0] = value;
            Assert.Equal(BitConverter.DoubleToInt64Bits(value), BitConverter.DoubleToInt64Bits(vector[0]));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Coordinates_SignedZero_PreservesSign(bool negative)
        {
            long bits = negative ? long.MinValue : 0;
            double value = BitConverter.Int64BitsToDouble(bits);
            var vector = Vector(value);
            Assert.Equal(bits, BitConverter.DoubleToInt64Bits(vector[0]));
            vector[0] = 1;
            vector[0] = value;
            Assert.Equal(bits, BitConverter.DoubleToInt64Bits(vector[0]));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void GetEnumerator_Coordinates_YieldsDoublesInOrder(int count)
        {
            double[] expected = new double[] { -1, 0, 0.25 }.Take(count).ToArray();
            var vector = new MathVector(expected);
            var actual = new List<double>();
            foreach (object value in vector)
            {
                actual.Add(Assert.IsType<double>(value));
            }
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumerator_RepeatedCalls_HaveIndependentPositions()
        {
            var vector = Vector(1, 2);
            IEnumerator first = vector.GetEnumerator();
            IEnumerator second = vector.GetEnumerator();
            Assert.True(first.MoveNext());
            Assert.True(first.MoveNext());
            Assert.True(second.MoveNext());
            Assert.Equal(2.0, first.Current);
            Assert.Equal(1.0, second.Current);
            Equal([1, 2], vector);
        }

        public static IEnumerable<object[]> LengthCases()
        {
            yield return [Array.Empty<double>(), 0.0, true];
            yield return [new double[] { 0, 0 }, 0.0, true];
            yield return [new double[] { 3 }, 3.0, true];
            yield return [new double[] { -3 }, 3.0, true];
            yield return [new double[] { 3, 4 }, 5.0, true];
            yield return [new double[] { 1e200 }, 1e200, true];
            yield return [new double[] { 3e200, 4e200 }, 5e200, false];
            yield return [new double[] { 3e-200, 4e-200 }, 5e-200, false];
            yield return [new double[] { double.Epsilon }, double.Epsilon, true];
            yield return [new double[] { double.MaxValue }, double.MaxValue, true];
        }

        [Theory]
        [MemberData(nameof(LengthCases))]
        public void Length_RepresentableNorm_ReturnsExpectedValue(double[] coordinates, double expected, bool exact)
        {
            double actual = new MathVector(coordinates).Length;
            if (exact)
            {
                Assert.Equal(expected, actual);
            }
            else
            {
                Close(expected, actual);
            }
        }

        [Fact]
        public void Length_UnrepresentableNorm_ThrowsArithmeticException()
        {
            var vector = Vector(double.MaxValue, double.MaxValue);
            Assert.Throws<ArithmeticException>(() => vector.Length);
            Equal([double.MaxValue, double.MaxValue], vector);
        }

        private sealed class SinglePassSource : IEnumerable
        {
            public int EnumerationCount { get; private set; }
            public IEnumerator GetEnumerator()
            {
                if (++EnumerationCount != 1)
                {
                    throw new InvalidOperationException("Повторное перечисление источника.");
                }
                return new double[] { 1, 2 }.GetEnumerator();
            }
        }
    }
}
