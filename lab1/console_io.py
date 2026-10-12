def askNumber(prompt, count):
    while True:
        try:
            number = int(input(prompt))
        except ValueError:
            print("Введите целое число.")
            continue
        if number < 1 or number > count:
            print(f"Введите номер от 1 до {count}.")
            continue
        if number == 67:
            print("сикс сэээвен")
        return number


def showRegions(regions):
    print("\nРегионы:")
    for index in range(len(regions)):
        print(f"{index + 1}: {regions[index]}")


def showColumns(columns):
    print("\nКолонки:")
    for index in range(len(columns)):
        print(f"{index + 1}: {columns[index]}")


def showStatistics(columnName, statistics):
    minimum, maximum, median, average, percentiles = statistics
    print(f"\nСтатистика колонки {columnName}:")
    print(f"Максимум: {maximum:.6g}")
    print(f"Минимум: {minimum:.6g}")
    print(f"Медиана: {median:.6g}")
    print(f"Среднее значение: {average:.6g}")
    percentileRows = []
    for percent, value in percentiles:
        percentileRows.append([str(percent), f"{value:.6g}"])
    print("\nПерцентили:")
    printTable(["Перцентиль", "Значение"], percentileRows)


def printTable(headers, rows):
    widths = []
    for column in range(len(headers)):
        width = len(headers[column])
        for row in rows:
            width = max(width, len(str(row[column])))
        widths.append(width)

    separator = "+"
    for width in widths:
        separator += "-" * (width + 2) + "+"
    print(separator)
    for row in [headers] + rows:
        line = "|"
        for column in range(len(headers)):
            line += " " + str(row[column]).ljust(widths[column]) + " |"
        print(line)
        if row is headers:
            print(separator)
    print(separator)
