using Xunit;
using static LinearAlgebra.Tests.VectorTestSupport;

namespace LinearAlgebra.Tests
{
    public class MathVectorArithmeticTests
    {
        public static IEnumerable<object[]> ScalarOperations() => Enum.GetValues<ScalarOperation>().Select(x => new object[] { x });
        public static IEnumerable<object[]> VectorOperations() => Enum.GetValues<VectorOperation>().Select(x => new object[] { x });
        public static IEnumerable<object[]> VectorResults() => Enum.GetValues<VectorOperation>()
            .Where(x => x is not (VectorOperation.ScalarMultiply or VectorOperation.Remainder or VectorOperation.Distance))
            .Select(x => new object[] { x });

        public static IEnumerable<object[]> ScalarCases()
        {
            foreach (ScalarOperation operation in Enum.GetValues<ScalarOperation>())
            {
                double[][] expected = operation switch
                {
                    ScalarOperation.SumNumber or ScalarOperation.Add => [[6, 0, 2, 2.5], [2, -4, -2, -1.5], [4.5, -1.5, 0.5, 1]],
                    ScalarOperation.Subtract => [[2, -4, -2, -1.5], [6, 0, 2, 2.5], [3.5, -2.5, -0.5, 0]],
                    ScalarOperation.MultiplyNumber or ScalarOperation.Multiply => [[8, -4, 0, 1], [-8, 4, 0, -1], [2, -1, 0, 0.25]],
                    _ => [[2, -1, 0, 0.25], [-2, 1, 0, -0.25], [8, -4, 0, 1]]
                };
                double[] numbers = [2, -2, 0.5];
                for (int i = 0; i < numbers.Length; i++)
                {
                    yield return [operation, numbers[i], expected[i]];
                }
            }
        }

        [Theory]
        [MemberData(nameof(ScalarCases))]
        public void ScalarOperation_FiniteScalar_ReturnsIndependentExpectedCoordinates(ScalarOperation operation, double number, double[] expected)
        {
            var source = Vector(4, -2, 0, 0.5);
            IMathVector result = Apply(operation, source, number);
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([4, -2, 0, 0.5], source);
            source[1] = 88;
            Assert.Equal(expected[1], result[1]);
        }

        [Theory]
        [InlineData(ScalarOperation.MultiplyNumber)]
        [InlineData(ScalarOperation.Multiply)]
        public void ScalarOperation_ZeroScalar_ReturnsExpectedCoordinates(ScalarOperation operation)
        {
            var vector = Vector(4, -2);
            IMathVector result = Apply(operation, vector, 0);
            Equal([0, 0], result);
            Assert.NotSame(vector, result);
            result[0] = 9;
            Equal([4, -2], vector);
        }

        [Theory]
        [MemberData(nameof(ScalarOperations))]
        public void ScalarOperation_NeutralScalar_ReturnsIndependentCopy(ScalarOperation operation)
        {
            var source = Vector(1, 2);
            double number = operation is ScalarOperation.SumNumber or ScalarOperation.Add or ScalarOperation.Subtract ? 0 : 1;
            IMathVector result = Apply(operation, source, number);
            Equal([1, 2], result);
            Assert.NotSame(source, result);
            result[0] = 8;
            Equal([1, 2], source);
        }

        [Theory]
        [MemberData(nameof(ScalarOperations))]
        public void ScalarOperation_EmptyVector_ReturnsNewEmptyVector(ScalarOperation operation)
        {
            var source = Vector();
            IMathVector result = Apply(operation, source, 2);
            Assert.NotSame(source, result);
            Equal([], result);
            Equal([], source);
        }

        public static IEnumerable<object[]> InvalidScalars()
        {
            foreach (object[] row in ScalarOperations())
            {
                foreach (double value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
                {
                    yield return [row[0], value];
                }
            }
        }

        [Theory]
        [MemberData(nameof(InvalidScalars))]
        public void ScalarOperation_NonFiniteScalar_ThrowsAndPreservesSource(ScalarOperation operation, double number)
        {
            var source = Vector(1, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => Apply(operation, source, number));
            Equal([1, 2], source);
        }

        [Theory]
        [MemberData(nameof(ScalarOperations))]
        public void ScalarOperation_Overflow_ThrowsAndPreservesSource(ScalarOperation operation)
        {
            var source = Vector(1, double.MaxValue);
            double number = operation switch
            {
                ScalarOperation.SumNumber or ScalarOperation.Add => double.MaxValue,
                ScalarOperation.Subtract => -double.MaxValue,
                ScalarOperation.MultiplyNumber or ScalarOperation.Multiply => 2,
                _ => 0.5
            };
            Assert.Throws<ArithmeticException>(() => Apply(operation, source, number));
            Equal([1, double.MaxValue], source);
        }

        [Theory]
        [InlineData(ScalarOperation.Add)]
        [InlineData(ScalarOperation.Subtract)]
        [InlineData(ScalarOperation.Multiply)]
        [InlineData(ScalarOperation.Divide)]
        public void ScalarOperator_NullLeft_ThrowsArgumentNullException(ScalarOperation operation)
        {
            Assert.Throws<ArgumentNullException>(() => Apply(operation, null!, 2));
        }

        public static IEnumerable<object[]> ZeroScalars()
        {
            foreach (ScalarOperation operation in new[] { ScalarOperation.DivideNumber, ScalarOperation.Divide })
            {
                foreach (int count in new[] { 0, 2 })
                {
                    yield return [operation, count, false];
                    yield return [operation, count, true];
                }
            }
        }

        [Theory]
        [MemberData(nameof(ZeroScalars))]
        public void DivideScalar_SignedZero_ThrowsAndPreservesSource(ScalarOperation operation, int count, bool negative)
        {
            double zero = BitConverter.Int64BitsToDouble(negative ? long.MinValue : 0);
            double[] coordinates = Enumerable.Repeat(1.0, count).ToArray();
            var source = new MathVector(coordinates);
            Assert.Throws<DivideByZeroException>(() => Apply(operation, source, zero));
            Equal(coordinates, source);
        }

        public static IEnumerable<object[]> VectorCases()
        {
            foreach (object[] row in VectorResults())
            {
                var operation = (VectorOperation)row[0];
                double[] expected = operation switch
                {
                    VectorOperation.Sum or VectorOperation.Add => [6, -6, 2, 2.5],
                    VectorOperation.Subtract => [2, 2, -2, -1.5],
                    VectorOperation.Multiply or VectorOperation.Product => [8, 8, 0, 1],
                    _ => [2, 0.5, 0, 0.25]
                };
                yield return [operation, expected];
            }
        }

        [Theory]
        [MemberData(nameof(VectorCases))]
        public void VectorOperation_MixedCoordinates_ReturnsIndependentExpectedResult(VectorOperation operation, double[] expected)
        {
            var left = Vector(4, -2, 0, 0.5);
            IMathVector right = Vector(2, -4, 2, 2);
            IMathVector result = Assert.IsAssignableFrom<IMathVector>(Apply(operation, left, right));
            Equal(expected, result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
            result[0] = 99;
            Equal([4, -2, 0, 0.5], left);
            Equal([2, -4, 2, 2], right);
            left[1] = 88;
            right[1] = 77;
            Assert.Equal(expected[1], result[1]);
        }

        public static IEnumerable<object[]> SingleCoordinateCases()
        {
            foreach (object[] row in VectorResults())
            {
                var operation = (VectorOperation)row[0];
                double expected = operation switch
                {
                    VectorOperation.Sum or VectorOperation.Add => 6,
                    VectorOperation.Subtract => 2,
                    VectorOperation.Multiply or VectorOperation.Product => 8,
                    _ => 2
                };
                yield return [operation, expected];
            }
        }

        [Theory]
        [MemberData(nameof(SingleCoordinateCases))]
        public void VectorOperation_SingleCoordinate_ReturnsExpectedResult(VectorOperation operation, double expected)
        {
            Equal([expected], Assert.IsAssignableFrom<IMathVector>(Apply(operation, Vector(4), Vector(2))));
        }

        [Theory]
        [MemberData(nameof(VectorResults))]
        public void VectorOperation_EmptyOperands_ReturnsNewEmptyVector(VectorOperation operation)
        {
            var left = Vector();
            var right = Vector();
            IMathVector result = Assert.IsAssignableFrom<IMathVector>(Apply(operation, left, right));
            Equal([], result);
            Assert.NotSame(left, result);
            Assert.NotSame(right, result);
        }

        public static IEnumerable<object[]> FractionalRightCases()
        {
            yield return [VectorOperation.Sum, new double[] { 4.5, -2.25 }];
            yield return [VectorOperation.Add, new double[] { 4.5, -2.25 }];
            yield return [VectorOperation.Subtract, new double[] { 3.5, -1.75 }];
            yield return [VectorOperation.Multiply, new double[] { 2, 0.5 }];
            yield return [VectorOperation.Product, new double[] { 2, 0.5 }];
            yield return [VectorOperation.Divide, new double[] { 8, 8 }];
            yield return [VectorOperation.Quotient, new double[] { 8, 8 }];
        }

        [Theory]
        [MemberData(nameof(FractionalRightCases))]
        public void VectorOperation_FractionalRightCoordinates_ReturnsExpectedResult(VectorOperation operation, double[] expected)
        {
            var left = Vector(4, -2);
            var right = Vector(0.5, -0.25);
            Equal(expected, Assert.IsAssignableFrom<IMathVector>(Apply(operation, left, right)));
        }

        [Theory]
        [InlineData(VectorOperation.Sum, 4, -2)]
        [InlineData(VectorOperation.Add, 4, -2)]
        [InlineData(VectorOperation.Subtract, 4, -2)]
        [InlineData(VectorOperation.Multiply, 0, 0)]
        [InlineData(VectorOperation.Product, 0, 0)]
        public void VectorOperation_ZeroRightCoordinates_ReturnsExpectedResult(VectorOperation operation, double first, double second)
        {
            Equal([first, second], Assert.IsAssignableFrom<IMathVector>(Apply(operation, Vector(4, -2), Vector(0, 0))));
        }

        public static IEnumerable<object[]> SelfCases()
        {
            foreach (object[] row in VectorResults())
            {
                var operation = (VectorOperation)row[0];
                double[] expected = operation switch
                {
                    VectorOperation.Sum or VectorOperation.Add => [4, 8],
                    VectorOperation.Subtract => [0, 0],
                    VectorOperation.Multiply or VectorOperation.Product => [4, 16],
                    _ => [1, 1]
                };
                yield return [operation, expected];
            }
        }

        [Theory]
        [MemberData(nameof(SelfCases))]
        public void VectorOperation_SameOperand_ReturnsIndependentResult(VectorOperation operation, double[] expected)
        {
            var source = Vector(2, 4);
            IMathVector result = Assert.IsAssignableFrom<IMathVector>(Apply(operation, source, source));
            Equal(expected, result);
            Assert.NotSame(source, result);
            result[0] = 99;
            Equal([2, 4], source);
        }

        public static IEnumerable<object[]> DimensionMismatches()
        {
            foreach (object[] row in VectorOperations())
            {
                yield return [row[0], 1];
                yield return [row[0], 3];
            }
        }

        [Theory]
        [MemberData(nameof(DimensionMismatches))]
        public void VectorOperation_DifferentDimensions_ThrowsAndPreservesOperands(VectorOperation operation, int rightCount)
        {
            var left = Vector(1, 2);
            double[] coordinates = Enumerable.Repeat(1.0, rightCount).ToArray();
            var right = new MathVector(coordinates);
            Assert.Throws<ArgumentException>(() => Apply(operation, left, right));
            Equal([1, 2], left);
            Equal(coordinates, right);
        }

        [Theory]
        [MemberData(nameof(VectorOperations))]
        public void VectorOperation_NullRight_ThrowsAndPreservesLeft(VectorOperation operation)
        {
            var left = Vector(1, 2);
            Assert.Throws<ArgumentNullException>(() => Apply(operation, left, null!));
            Equal([1, 2], left);
        }

        [Theory]
        [InlineData(VectorOperation.Add)]
        [InlineData(VectorOperation.Subtract)]
        [InlineData(VectorOperation.Product)]
        [InlineData(VectorOperation.Quotient)]
        [InlineData(VectorOperation.Remainder)]
        public void VectorOperator_NullLeft_ThrowsArgumentNullException(VectorOperation operation)
        {
            Assert.Throws<ArgumentNullException>(() => Apply(operation, null!, Vector(1)));
        }

        [Theory]
        [MemberData(nameof(VectorCases))]
        public void VectorOperation_ForeignImplementation_ReturnsExpectedResult(VectorOperation operation, double[] expected)
        {
            var left = Vector(4, -2, 0, 0.5);
            IMathVector right = new ForeignVector(2, -4, 2, 2);
            IMathVector result = Assert.IsAssignableFrom<IMathVector>(Apply(operation, left, right));
            Equal(expected, result);
            result[0] = 99;
            Equal([4, -2, 0, 0.5], left);
            Equal([2, -4, 2, 2], right);
        }

        public static IEnumerable<object[]> InvalidForeignCoordinates()
        {
            foreach (object[] row in VectorOperations())
            {
                foreach (double value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
                {
                    yield return [row[0], value];
                }
            }
        }

        [Theory]
        [MemberData(nameof(InvalidForeignCoordinates))]
        public void VectorOperation_NonFiniteForeignCoordinate_ThrowsAndPreservesOperands(VectorOperation operation, double value)
        {
            var left = Vector(1, 2);
            IMathVector right = new ForeignVector(1, value);
            Assert.Throws<ArgumentException>(() => Apply(operation, left, right));
            Equal([1, 2], left);
            Assert.Equal(1, right[0]);
            Assert.Equal(value, right[1]);
        }

        public static IEnumerable<object[]> ZeroCoordinateCases()
        {
            foreach (VectorOperation operation in new[] { VectorOperation.Divide, VectorOperation.Quotient })
            {
                foreach (int index in new[] { 0, 1, 2 })
                {
                    yield return [operation, index, false];
                    yield return [operation, index, true];
                }
            }
        }

        [Theory]
        [MemberData(nameof(ZeroCoordinateCases))]
        public void DivideVector_ZeroCoordinate_ThrowsAndPreservesOperands(VectorOperation operation, int index, bool negative)
        {
            double zero = BitConverter.Int64BitsToDouble(negative ? long.MinValue : 0);
            var left = Vector(1, 2, 3);
            double[] coordinates = [1, 1, 1];
            coordinates[index] = zero;
            var right = new MathVector(coordinates);
            Assert.Throws<DivideByZeroException>(() => Apply(operation, left, right));
            Equal([1, 2, 3], left);
            Equal(coordinates, right);
        }

        [Theory]
        [MemberData(nameof(VectorOperations))]
        public void VectorOperation_Overflow_ThrowsAndPreservesOperands(VectorOperation operation)
        {
            var left = Vector(1, double.MaxValue);
            double second = operation switch
            {
                VectorOperation.Sum or VectorOperation.Add => double.MaxValue,
                VectorOperation.Subtract or VectorOperation.Distance => -double.MaxValue,
                VectorOperation.Divide or VectorOperation.Quotient => 0.5,
                _ => 2
            };
            var right = Vector(1, second);
            Assert.Throws<ArithmeticException>(() => Apply(operation, left, right));
            Equal([1, double.MaxValue], left);
            Equal([1, second], right);
        }
    }
}
