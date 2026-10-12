def getRegions(rows, regionId):
    regions = []
    for row in rows:
        region = row[regionId]
        if region not in regions:
            regions.append(region)
    return sorted(regions)


def selectRegion(rows, regionId, region):
    selected = []
    for row in rows:
        if row[regionId] == region:
            selected.append(row)
    if not selected:
        raise ValueError("Регион не найден. Проверьте название.")
    return selected


def getNumbers(rows, columnId):
    numbers = []
    for index in range(len(rows)):
        cell = rows[index][columnId]
        if not cell:
            continue
        try:
            number = float(cell)
        except ValueError:
            raise ValueError(
                f"Строка региона {index + 1}: значение '{cell}' не является числом."
            )
        if number != number or number == float("inf") or number == -float("inf"):
            raise ValueError(
                f"Строка региона {index + 1}: число должно быть конечным."
            )
        numbers.append(number)
    return numbers


def percentile(numbers, percent):
    position = (len(numbers) - 1) * percent / 100
    left = int(position)
    if left == len(numbers) - 1:
        return numbers[left]
    fraction = position - left
    return numbers[left] * (1 - fraction) + numbers[left + 1] * fraction


def calculateStatistics(numbers):
    if not numbers:
        raise ValueError("Нет чисел для расчёта.")
    numbers = sorted(numbers)
    average = 0
    for number in numbers:
        average += number / len(numbers)
    if average == float("inf") or average == -float("inf"):
        raise ValueError("Числа слишком велики для вычисления среднего.")

    percentiles = []
    for percent in range(0, 101, 5):
        percentiles.append([percent, percentile(numbers, percent)])
    return numbers[0], numbers[-1], percentile(numbers, 50), average, percentiles
