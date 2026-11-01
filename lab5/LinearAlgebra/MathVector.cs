using System.Collections;

namespace LinearAlgebra
{
    /// <summary>Вектор с фиксированной размерностью и изменяемыми конечными координатами.</summary>
    public class MathVector : IEnumerable
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

                // Масштабирование сохраняет представимую норму при больших и малых координатах.
                double sum = 0;
                foreach (double coordinate in _coordinates)
                {
                    double normalized = coordinate / scale;
                    sum += normalized * normalized;
                }

                double length = scale * Math.Sqrt(sum);
                if (!double.IsFinite(length))
                {
                    throw new ArithmeticException("Длина вектора не представима конечным double.");
                }

                return length;
            }
        }

        /// <summary>Перечисляет координаты как double в исходном порядке.</summary>
        public IEnumerator GetEnumerator()
        {
            return _coordinates.GetEnumerator();
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
