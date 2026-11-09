using System.Collections;

namespace LinearAlgebra
{
    /// <summary>Вектор с фиксированной размерностью и изменяемыми конечными координатами.</summary>
    /// <remarks>
    /// Арифметические операции сохраняют операнды и создают независимые векторные результаты.
    /// Null-операнд вызывает ArgumentNullException, разные размерности или неконечные
    /// координаты другого вектора — ArgumentException, неконечный скаляр — ArgumentOutOfRangeException.
    /// Нулевой делитель вызывает DivideByZeroException, неконечный результат — ArithmeticException.
    /// </remarks>
    public class MathVector : IMathVector
    {
        private readonly double[] _coordinates;

        /// <summary>
        /// Создаёт независимую копию координат, перечисляя источник один раз.
        /// Пустой источник задаёт вектор размерности ноль.
        /// </summary>
        /// <exception cref="ArgumentNullException">Источник равен null.</exception>
        /// <exception cref="ArgumentException">Элемент не является конечным double.</exception>
        public MathVector(IEnumerable coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            var values = new List<double>();
            foreach (object? item in coordinates)
            {
                if (item is not double coordinate || !double.IsFinite(coordinate))
                {
                    throw new ArgumentException("Координаты должны быть конечными значениями double.", nameof(coordinates));
                }

                values.Add(coordinate);
            }

            _coordinates = values.ToArray();
        }

        /// <summary>Количество координат, неизменное после создания вектора.</summary>
        public int Dimensions => _coordinates.Length;

        /// <summary>Доступ к координате по индексу, начиная с нуля.</summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Индекс вне границ вектора или записываемое значение не является конечным.
        /// При отказе записи координаты сохраняются.
        /// </exception>
        public double this[int i]
        {
            get
            {
                ValidateIndex(i);
                return _coordinates[i];
            }
            set
            {
                ValidateIndex(i);
                if (!double.IsFinite(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Координата должна быть конечной.");
                }

                _coordinates[i] = value;
            }
        }

        /// <summary>Евклидова длина; для пустого и нулевого вектора равна нулю.</summary>
        /// <exception cref="ArithmeticException">Длина не представима конечным double.</exception>
        public double Length
        {
            get
            {
                double scale = 0;
                foreach (double coordinate in _coordinates)
                {
                    scale = Math.Max(scale, Math.Abs(coordinate));
                }

                if (scale == 0)
                {
                    return 0;
                }

                double sum = 0;
                foreach (double coordinate in _coordinates)
                {
                    double normalized = coordinate / scale;
                    sum += normalized * normalized;
                }

                double length = scale * Math.Sqrt(sum);
                return RequireFiniteResult(length);
            }
        }

        /// <summary>Перечисляет координаты как double в исходном порядке.</summary>
        public IEnumerator GetEnumerator()
        {
            return _coordinates.GetEnumerator();
        }

        /// <summary>Покомпонентно прибавляет число, возвращая независимый вектор.</summary>
        public IMathVector SumNumber(double number)
        {
            return ApplyNumber(number, (coordinate, scalar) => coordinate + scalar);
        }

        /// <summary>Покомпонентно умножает на число, возвращая независимый вектор.</summary>
        public IMathVector MultiplyNumber(double number)
        {
            return ApplyNumber(number, (coordinate, scalar) => coordinate * scalar);
        }

        /// <summary>Делит координаты на конечное ненулевое число.</summary>
        public IMathVector DivideNumber(double number)
        {
            ValidateNumber(number);
            ValidateDivisor(number);
            return ApplyNumber(number, (coordinate, scalar) => coordinate / scalar);
        }

        /// <summary>Складывает векторы одинаковой размерности, создавая независимый результат.</summary>
        public IMathVector Sum(IMathVector vector)
        {
            return ApplyVector(vector, (left, right) => left + right);
        }

        /// <summary>Покомпонентно умножает векторы одинаковой размерности.</summary>
        public IMathVector Multiply(IMathVector vector)
        {
            return ApplyVector(vector, (left, right) => left * right);
        }

        /// <summary>Покомпонентно делит на вектор с ненулевыми координатами.</summary>
        public IMathVector Divide(IMathVector vector)
        {
            double[] coordinates = ReadCoordinates(vector);
            foreach (double coordinate in coordinates)
            {
                ValidateDivisor(coordinate);
            }

            return ApplyCoordinates(coordinates, (left, right) => left / right);
        }

        /// <summary>Возвращает сумму произведений соответствующих координат.</summary>
        public double ScalarMultiply(IMathVector vector)
        {
            double[] coordinates = ReadCoordinates(vector);
            double sum = 0;
            for (int i = 0; i < Dimensions; i++)
            {
                double product = RequireFiniteResult(_coordinates[i] * coordinates[i]);
                sum = RequireFiniteResult(sum + product);
            }

            return sum;
        }

        /// <summary>Возвращает евклидову длину разности векторов одинаковой размерности.</summary>
        public double CalcDistance(IMathVector vector)
        {
            return ApplyVector(vector, (left, right) => left - right).Length;
        }

        // ===== Перегрузка операторов =====

        /// <summary>Покомпонентное сложение с числом.</summary>
        public static IMathVector operator +(MathVector left, double number)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.SumNumber(number);
        }

        /// <summary>Покомпонентное вычитание числа.</summary>
        public static IMathVector operator -(MathVector left, double number)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.SumNumber(-number);
        }

        /// <summary>Покомпонентное умножение на число.</summary>
        public static IMathVector operator *(MathVector left, double number)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.MultiplyNumber(number);
        }

        /// <summary>Покомпонентное деление на число.</summary>
        public static IMathVector operator /(MathVector left, double number)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.DivideNumber(number);
        }

        /// <summary>Покомпонентное сложение векторов.</summary>
        public static IMathVector operator +(MathVector left, IMathVector right)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.Sum(right);
        }

        /// <summary>Покомпонентное вычитание векторов.</summary>
        public static IMathVector operator -(MathVector left, IMathVector right)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.Sum(new MathVector(left.ReadCoordinates(right)).MultiplyNumber(-1));
        }

        /// <summary>Покомпонентное умножение векторов.</summary>
        public static IMathVector operator *(MathVector left, IMathVector right)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.Multiply(right);
        }

        /// <summary>Покомпонентное деление векторов.</summary>
        public static IMathVector operator /(MathVector left, IMathVector right)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.Divide(right);
        }

        /// <summary>Скалярное произведение векторов.</summary>
        public static double operator %(MathVector left, IMathVector right)
        {
            ArgumentNullException.ThrowIfNull(left);
            return left.ScalarMultiply(right);
        }

        // ===== Вспомогательные методы =====

        private IMathVector ApplyNumber(double number, Func<double, double, double> operation)
        {
            ValidateNumber(number);
            var result = new double[Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                result[i] = RequireFiniteResult(operation(_coordinates[i], number));
            }

            return new MathVector(result);
        }

        private IMathVector ApplyVector(IMathVector vector, Func<double, double, double> operation)
        {
            return ApplyCoordinates(ReadCoordinates(vector), operation);
        }

        private IMathVector ApplyCoordinates(double[] coordinates, Func<double, double, double> operation)
        {
            var result = new double[Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                result[i] = RequireFiniteResult(operation(_coordinates[i], coordinates[i]));
            }

            return new MathVector(result);
        }

        private double[] ReadCoordinates(IMathVector vector)
        {
            ArgumentNullException.ThrowIfNull(vector);
            if (vector.Dimensions != Dimensions)
            {
                throw new ArgumentException("Размерности векторов должны совпадать.", nameof(vector));
            }

            var coordinates = new double[Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                double coordinate = vector[i];
                if (!double.IsFinite(coordinate))
                {
                    throw new ArgumentException("Координаты вектора должны быть конечными.", nameof(vector));
                }

                coordinates[i] = coordinate;
            }

            return coordinates;
        }

        // ===== Валидаторы =====

        private static void ValidateNumber(double number)
        {
            if (!double.IsFinite(number))
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Число должно быть конечным.");
            }
        }

        private static void ValidateDivisor(double divisor)
        {
            if (divisor == 0)
            {
                throw new DivideByZeroException("Делитель не может быть равен нулю.");
            }
        }

        private static double RequireFiniteResult(double result)
        {
            if (!double.IsFinite(result))
            {
                throw new ArithmeticException("Результат арифметики не представим конечным double.");
            }

            return result;
        }

        private void ValidateIndex(int i)
        {
            if (i < 0 || i >= Dimensions)
            {
                throw new ArgumentOutOfRangeException(nameof(i), i, "Индекс должен находиться в границах вектора.");
            }
        }
    }
}
