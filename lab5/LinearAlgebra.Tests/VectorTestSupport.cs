using System.Collections;
using Xunit;

namespace LinearAlgebra.Tests
{
    internal static class VectorTestSupport
    {
        internal static MathVector Vector(params double[] coordinates) => new(coordinates);

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
