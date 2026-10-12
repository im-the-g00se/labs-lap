# Языки прикладного программирования

## Лабораторная работа №2 «Визуализация данных при помощи Python»

### Задание 1

Взять датасет ([ссылка](https://www.kaggle.com/datasets/ibrahimonmars/global-cargo-ships-dataset?resource=download&select=Cleaned_ships_data.csv), файл Cleaned_ships_data.csv) и в jupyter notebook визуализировать при помощи графиков:

- 2-D график зависимости средней максимальной загрузки кораблей от длины корабля
- Распределение количества кораблей по их типу (ship_name). В целях более корректного отображения диаграммы типы, по которым меньше всего кораблей, можно объединить в один сегмент с названием «Другие».
- Гистограмма распределения количества кораблей по годам постройки (built_year)
- 2-D Гистограмма распределения количества кораблей по годам постройки (built_year) и максимальному весу груза (dwt). Для максимального веса груза сформировать 20 диапазонов, от минимального до максимального значений с равным шагом.

Взять датасет ([ссылка](https://www.kaggle.com/datasets/arshid/iris-flower-dataset)) и в jupyter notebook визуализировать при помощи графиков:

- Парную диаграмму (Pairplot) для датасета
- Скрипичную диаграмму (Violinplot) с распределением всех четырех характеристик ирисов для каждого вида (для каждой характеристики отдельные «скрипки» для разных видов ирисов)

### Задание 2

Выбрать на kaggle любой датасет, например, один из:

- https://www.kaggle.com/datasets/spscientist/students-performance-in-exams
- https://www.kaggle.com/datasets/mohammadtalib786/pubg-stats-dataset
- https://www.kaggle.com/datasets/fedesoriano/heart-failure-prediction
- https://www.kaggle.com/datasets/joebeachcapital/fortnite-statistics
- https://www.kaggle.com/datasets/mrpantherson/board-game-data
- https://www.kaggle.com/datasets/mabdullahsajid/global-grain-and-coffee-price-history-1973-2023
- https://www.kaggle.com/datasets/harishkumardatalab/data-science-salary-2021-to-2023
- https://www.kaggle.com/datasets/shashankshukla123123/marketing-campaign
- https://www.kaggle.com/datasets/bhuviranga/customer-segmentation

Самостоятельно визуализировать данные из датасета, использовав не менее трех различных графиков/гистограмм. Выписать (в виде текстового блока под каждой из диаграмм) что визуализировали и выводы, которые вы сделали на основании графика о данных.

Быть готовым объяснить преподавателю все вышеописанное.

### Общие требования

- На каждом графике должна быть «легенда»
- Следовать принципам KISS, DRY, YAGNI и т.п.
- Код должен соответствовать code-style соответствующего языка: [для Python](https://google.github.io/styleguide/pyguide.html)

### Полезные ссылки

- https://jakevdp.github.io/PythonDataScienceHandbook/index.html
- https://www.kaggle.com/datasets
