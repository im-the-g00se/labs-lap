using System.Collections;
using Xunit;

namespace LinearAlgebra.Tests
{
    public enum ScalarOperation
    {
        SumNumber, MultiplyNumber, DivideNumber, Add, Subtract, Multiply, Divide
    }

    public enum VectorOperation
    {
        Sum, Multiply, Divide, Add, Subtract, Product, Quotient, ScalarMultiply, Remainder, Distance
    }

    internal static class VectorTestSupport
    {
        internal static MathVector Vector(params double[] coordinates) => new(coordinates);

        internal static IMathVector Apply(ScalarOperation operation, MathVector vector, double number) => operation switch
        {
            ScalarOperation.SumNumber => vector.SumNumber(number),
            ScalarOperation.MultiplyNumber => vector.MultiplyNumber(number),
            ScalarOperation.DivideNumber => vector.DivideNumber(number),
            ScalarOperation.Add => vector + number,
            ScalarOperation.Subtract => vector - number,
            ScalarOperation.Multiply => vector * number,
            ScalarOperation.Divide => vector / number,
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

        internal static object Apply(VectorOperation operation, MathVector left, IMathVector right) => operation switch
        {
            VectorOperation.Sum => left.Sum(right),
            VectorOperation.Multiply => left.Multiply(right),
            VectorOperation.Divide => left.Divide(right),
            VectorOperation.Add => left + right,
            VectorOperation.Subtract => left - right,
            VectorOperation.Product => left * right,
            VectorOperation.Quotient => left / right,
            VectorOperation.ScalarMultiply => left.ScalarMultiply(right),
            VectorOperation.Remainder => left % right,
            VectorOperation.Distance => left.CalcDistance(right),
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

        internal static void Equal(IEnumerable<double> expected, IMathVector actual)
        {
            Assert.Equal(expected, actual.Cast<double>());
        }

        internal static void Close(double expected, double actual)
        {
            Assert.True(double.IsFinite(actual));
            // Относительный допуск не позволяет заменить малое ненулевое значение нулём.
            Assert.InRange(Math.Abs((actual - expected) / expected), 0, 1e-14);
        }
    }

    // Отдельная реализация контракта, не использующая MathVector для вычислений.
    internal sealed class ForeignVector(params double[] coordinates) : IMathVector
    {
        private readonly double[] _coordinates = (double[])coordinates.Clone();
        public int Dimensions => _coordinates.Length;
        public double this[int i] { get => _coordinates[i]; set => _coordinates[i] = value; }
        public double Length => Math.Sqrt(_coordinates.Sum(x => x * x));
        public IEnumerator GetEnumerator() => _coordinates.GetEnumerator();
        public IMathVector SumNumber(double number) => new ForeignVector(_coordinates.Select(x => x + number).ToArray());
        public IMathVector MultiplyNumber(double number) => new ForeignVector(_coordinates.Select(x => x * number).ToArray());
        public IMathVector DivideNumber(double number) => new ForeignVector(_coordinates.Select(x => x / number).ToArray());
        public IMathVector Sum(IMathVector vector) => new ForeignVector(_coordinates.Select((x, i) => x + vector[i]).ToArray());
        public IMathVector Multiply(IMathVector vector) => new ForeignVector(_coordinates.Select((x, i) => x * vector[i]).ToArray());
        public IMathVector Divide(IMathVector vector) => new ForeignVector(_coordinates.Select((x, i) => x / vector[i]).ToArray());
        public double ScalarMultiply(IMathVector vector) => _coordinates.Select((x, i) => x * vector[i]).Sum();
        public double CalcDistance(IMathVector vector) => Math.Sqrt(_coordinates.Select((x, i) => (x - vector[i]) * (x - vector[i])).Sum());
    }
}
