import csv

from console_io import askNumber, printTable, showColumns, showRegions, showStatistics
from file_read import readData
from logic import calculateStatistics, getNumbers, getRegions, selectRegion


def main():
    defaultPath = "sample_csv/russian_demography.csv"
    while True:
        path = input(f"Путь к CSV-файлу (Enter — {defaultPath}): ").strip()
        if not path:
            path = defaultPath
        try:
            headers, rows = readData(path)
            break
        except (OSError, UnicodeError, csv.Error, ValueError) as error:
            print(f"Не удалось прочитать файл: {error}")

    regionId = headers.index("region")
    regions = getRegions(rows, regionId)
    showRegions(regions)
    regionNumber = askNumber("Номер региона: ", len(regions))
    region = regions[regionNumber - 1]
    selected = selectRegion(rows, regionId, region)

    print("\nДанные выбранного региона:")
    printTable(headers, selected)

    columnIds = []
    columns = []
    for index in range(len(headers)):
        if index != regionId and headers[index] != "year":
            columnIds.append(index)
            columns.append(headers[index])
    if not columnIds:
        print("В файле нет колонок для расчёта статистики.")
        return
    showColumns(columns)

    while True:
        columnNumber = askNumber("ID числовой колонки: ", len(columnIds))
        columnId = columnIds[columnNumber - 1]
        try:
            numbers = getNumbers(selected, columnId)
            statistics = calculateStatistics(numbers)
            break
        except ValueError as error:
            print(f"Ошибка выбора или данных: {error}")

    showStatistics(headers[columnId], statistics)


if __name__ == "__main__":
    try:
        main()
    except (KeyboardInterrupt, EOFError):
        print("\nРабота программы завершена.")
