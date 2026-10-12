import csv

def readHeaders(reader):
    headers = next(reader, None)
    if not headers:
        raise ValueError("Файл пуст или первая строка не содержит заголовков.")
    headers = [name.strip() for name in headers]
    for name in headers:
        if not name or headers.count(name) > 1:
            raise ValueError("Заголовки должны быть непустыми и уникальными.")
    if "region" not in headers:
        raise ValueError("В файле отсутствует колонка region.")
    return headers


def readRows(reader, headers):
    rows = []
    for row in reader:
        row = [cell.strip() for cell in row]
        if len(row) != len(headers):
            continue
        rows.append(row)
    if not rows:
        raise ValueError("В файле нет строк с данными.")
    return rows


def readData(path):
    with open(path, encoding="utf-8-sig", newline="") as file:
        reader = csv.reader(file, strict=True)
        headers = readHeaders(reader)
        rows = readRows(reader, headers)
    return headers, rows
