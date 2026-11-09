using System.Globalization;
using LinearAlgebra;

namespace VectorDemo
{
    internal static class Program
    {
        private static void Main()
        {
            IMathVector first = new MathVector(new double[] { 3, 4 });
            IMathVector second = new MathVector(new List<double> { 1, 2 });
            IMathVector copy = new MathVector(first);

            Console.WriteLine("Создание и базовые свойства");
            ShowVector("a из массива", first);
            ShowVector("b из списка", second);
            ShowVector("Копия a из IMathVector", copy);
            Console.WriteLine($"a.Dimensions = {first.Dimensions}");
            ShowNumber("a.Length", first.Length);
            ShowNumber("Копия a: [0] до записи", copy[0]);
            copy[0] = 9;
            ShowVector("Копия a после записи 9 в [0]", copy);
            ShowVector("Исходный a после изменения копии", first);

            Console.WriteLine("Координаты a через foreach:");
            foreach (double coordinate in first)
            {
                Console.WriteLine($"  {FormatNumber(coordinate)}");
            }

            Console.WriteLine();
            Console.WriteLine("Методы: a = [3, 4], b = [1, 2], число = 2");
            ShowVector("a.SumNumber(2)", first.SumNumber(2));
            ShowVector("a.MultiplyNumber(2)", first.MultiplyNumber(2));
            ShowVector("a.DivideNumber(2)", first.DivideNumber(2));
            ShowVector("a.Sum(b)", first.Sum(second));
            ShowVector("a.Multiply(b), покомпонентно", first.Multiply(second));
            ShowVector("a.Divide(b), покомпонентно", first.Divide(second));
            ShowNumber("a.ScalarMultiply(b), скалярное произведение", first.ScalarMultiply(second));
            ShowNumber("a.CalcDistance(b)", first.CalcDistance(second));

            Console.WriteLine();
            Console.WriteLine("Операторы: a = [3, 4], b = [1, 2], число = 2");
            ShowVector("a + 2", (MathVector)first + 2);
            ShowVector("a - 2", (MathVector)first - 2);
            ShowVector("a * 2", (MathVector)first * 2);
            ShowVector("a / 2", (MathVector)first / 2);
            ShowVector("a + b", (MathVector)first + second);
            ShowVector("a - b", (MathVector)first - second);
            ShowVector("a * b, покомпонентно", (MathVector)first * second);
            ShowVector("a / b, покомпонентно", (MathVector)first / second);
            ShowNumber("a % b, скалярное произведение", (MathVector)first % second);

            Console.WriteLine();
            Console.WriteLine("Независимость результата");
            IMathVector result = first.Sum(second);
            ShowVector("Результат a.Sum(b) до записи", result);
            result[0] = 99;
            ShowVector("Результат после записи 99 в [0]", result);
            ShowVector("Исходный a", first);
            ShowVector("Исходный b", second);

            Console.WriteLine();
            Console.WriteLine("Пустой вектор");
            IMathVector empty = new MathVector(Array.Empty<double>());
            ShowVector("empty", empty);
            Console.WriteLine($"empty.Dimensions = {empty.Dimensions}");
            ShowNumber("empty.Length", empty.Length);
            ShowVector("empty.Sum(empty)", empty.Sum(empty));

            Console.WriteLine();
            Console.WriteLine("Ожидаемые ошибки");
            ShowExpectedError<ArgumentOutOfRangeException>("Запись a[-1] = 10", () => first[-1] = 10);
            ShowExpectedError<ArgumentException>("a.Sum([1]): разные размерности", () => first.Sum(new MathVector(new double[] { 1 })));
            ShowExpectedError<DivideByZeroException>("a.DivideNumber(0)", () => first.DivideNumber(0));
            ShowExpectedError<DivideByZeroException>("a.Divide([1, 0])", () => first.Divide(new MathVector(new double[] { 1, 0 })));
            ShowExpectedError<ArgumentException>("Создание вектора [NaN]", () => _ = new MathVector(new double[] { double.NaN }));
            ShowExpectedError<ArgumentOutOfRangeException>("a.SumNumber(+Infinity)", () => first.SumNumber(double.PositiveInfinity));
            ShowExpectedError<ArithmeticException>("a.MultiplyNumber(double.MaxValue): переполнение", () => first.MultiplyNumber(double.MaxValue));

            Console.WriteLine();
            ShowVector("a", first);
            ShowVector("b", second);
            ShowVector("a.Sum(b)", first.Sum(second));
        }

        private static void ShowVector(string label, IMathVector vector)
        {
            string coordinates = string.Join(", ", vector.Cast<double>().Select(FormatNumber));
            Console.WriteLine($"{label} = [{coordinates}]");
        }

        private static void ShowNumber(string label, double number)
        {
            Console.WriteLine($"{label} = {FormatNumber(number)}");
        }

        private static string FormatNumber(double number)
        {
            return number.ToString("G", CultureInfo.InvariantCulture);
        }

        private static void ShowExpectedError<TException>(string label, Action action) where TException : Exception
        {
            Console.WriteLine($"{label}:");
            try
            {
                action();
            }
            catch (TException exception)
            {
                Console.WriteLine($"  {typeof(TException).Name}: {exception.Message}");
                return;
            }

            throw new InvalidOperationException($"Сценарий «{label}» не вызвал ожидаемое исключение {typeof(TException).Name}.");
        }
    }
}
