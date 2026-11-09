using System.Collections;
using Xunit;
using static LinearAlgebra.Tests.VectorTestSupport;

namespace LinearAlgebra.Tests
{
    public class MathVectorCoreTests
    {
        [Fact]
        public void Constructor_SingleElementArray_CopiesCoordinatesAndSetsDimensions()
        {
            double[] source = [-0.25];
            var vector = new MathVector(source);
            Assert.Equal(1, vector.Dimensions);
            source[0] = 99;
            Equal([-0.25], vector);
            vector[0] = 88;
            Equal([88], vector);
            Assert.Equal([99], source);
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

        [Fact]
        public void Constructor_MathVector_CopiesIndependently()
        {
            var source = Vector(1, 2);
            var copy = new MathVector(source);
            source[0] = 99;
            copy[1] = 88;
            Equal([1, 88], copy);
            Equal([99, 2], source);
        }

        [Fact]
        public void Constructor_IMathVector_CopiesCoordinates()
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
        public void Constructor_EmptySource_CreatesEmptyVector()
        {
            var vector = new MathVector(Array.Empty<double>());
            Assert.Equal(0, vector.Dimensions);
            Equal([], vector);
        }

        [Fact]
        public void Constructor_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MathVector(null!));
        }

        [Fact]
        public void Constructor_IntegerElement_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MathVector(new object?[] { 1.0, 1 }));
        }

        [Fact]
        public void Constructor_NullElement_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MathVector(new object?[] { 1.0, null }));
        }

        [Fact]
        public void Constructor_NaN_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MathVector(new object?[] { 1.0, double.NaN }));
        }

        [Fact]
        public void IndexerGet_FirstIndex_ReturnsSelectedCoordinate()
        {
            var vector = Vector(1, 2, 3);
            Assert.Equal(1, vector[0]);
        }

        [Fact]
        public void IndexerGet_InteriorIndex_ReturnsSelectedCoordinate()
        {
            var vector = Vector(1, 2, 3);
            Assert.Equal(2, vector[1]);
        }

        [Fact]
        public void IndexerGet_LastIndex_ReturnsSelectedCoordinate()
        {
            var vector = Vector(1, 2, 3);
            Assert.Equal(3, vector[2]);
        }

        [Fact]
        public void IndexerSet_FirstIndex_UpdatesOnlySelectedCoordinate()
        {
            var vector = Vector(1, 2, 3);
            vector[0] = 8;
            double[] expected = [1, 2, 3];
            expected[0] = 8;
            Equal(expected, vector);
            Assert.Equal(3, vector.Dimensions);
        }

        [Fact]
        public void IndexerSet_LastIndex_UpdatesOnlySelectedCoordinate()
        {
            var vector = Vector(1, 2, 3);
            vector[2] = 8;
            double[] expected = [1, 2, 3];
            expected[2] = 8;
            Equal(expected, vector);
            Assert.Equal(3, vector.Dimensions);
        }

        [Fact]
        public void Indexer_NegativeIndex_ReadThrowsArgumentOutOfRangeException()
        {
            var vector = new MathVector(new double[3]);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector[-1]);
        }

        [Fact]
        public void Indexer_NegativeIndex_WriteThrowsAndPreservesCoordinates()
        {
            var vector = new MathVector(new double[3]);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector[-1] = 8);
            Equal(new double[3], vector);
        }

        [Fact]
        public void Indexer_IndexEqualToDimensions_ReadThrowsArgumentOutOfRangeException()
        {
            var vector = new MathVector(new double[3]);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector[3]);
        }

        [Fact]
        public void Indexer_IndexEqualToDimensions_WriteThrowsAndPreservesCoordinates()
        {
            var vector = new MathVector(new double[3]);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector[3] = 8);
            Equal(new double[3], vector);
        }

        [Fact]
        public void Indexer_PositiveInfinity_ThrowsAndPreservesCoordinates()
        {
            var vector = Vector(1, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => vector[0] = double.PositiveInfinity);
            Equal([1, 2], vector);
        }

        [Fact]
        public void GetEnumerator_EmptyVector_YieldsDoublesInOrder()
        {
            double[] expected = [];
            var vector = new MathVector(expected);
            var actual = new List<double>();
            foreach (object value in vector)
            {
                actual.Add(Assert.IsType<double>(value));
            }
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumerator_MultipleCoordinates_YieldsDoublesInOrder()
        {
            double[] expected = [-1, 0, 0.25];
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

        [Fact]
        public void Length_EmptyVector_ReturnsExpectedValue()
        {
            double actual = Vector().Length;
            Assert.Equal(0.0, actual);
        }

        [Fact]
        public void Length_ZeroVector_ReturnsExpectedValue()
        {
            double actual = Vector(0, 0).Length;
            Assert.Equal(0.0, actual);
        }

        [Fact]
        public void Length_MultipleCoordinates_ReturnsExpectedValue()
        {
            double actual = Vector(3, 4).Length;
            Assert.Equal(5.0, actual);
        }

        [Fact]
        public void Length_LargeCoordinates_ReturnsExpectedValue()
        {
            double actual = Vector(3e200, 4e200).Length;
            Close(5e200, actual);
        }

        [Fact]
        public void Length_SmallCoordinates_ReturnsExpectedValue()
        {
            double actual = Vector(3e-200, 4e-200).Length;
            Close(5e-200, actual);
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
