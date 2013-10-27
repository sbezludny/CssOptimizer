CssOptimizer
------------

### Описание

Консольное приложение для поиска неиспользуемых css-стилей на странице. 

### Возможности

- Анализ всех css-файлов доступных на странице.
	- Css-файлы подключенные через тег ```<link>```
	- Css внутри тега ```<style>```
	
	
- Анализ нескольких страниц
- Анализ всего сайта с возможностью задания к-ва страниц
- Задание глубины вложенности при анализе сайта

##### В разработке
- Анализ css-файлов подключаемых через ```@import```

### Поддерживаемые селекторы

- Универсальный селектор (Universal selectors)
- Селекторы тегов (Type selectors)
- Селекторы атрибутов (Attribute selectors)
- Идентификаторы (ID selectors)
- Классы (Class selectors)
- Вложенные селекторы (Descendant selectors)
- Дочерние селекторы (Child selectors)
- Соседние селекторы (Adjacent sibling selectors)
- Родственные селекторы (General sibling selectors)
- Частичная поддержка псевдоклассов ``` :first-child, :last-child, :only-child, :nth-child, :empty, :not, :contains, :disabled, :checked ```

#### Синтаксис 

	> ./CssOptimizer.App.exe <url1> [url2…] [-s] [-c max_pages] [-d max_depth] [-q] [-o output_file]

#### Описание

- ```url1 [url2…]``` Адреса анализируемых страниц.

- ```-s``` Режим анализа сайта.

- ```-с max_pages``` Ограничение количества страниц при анализе сайта.

- ```-d max_depth``` Ограничение глубины сканирования при анализе сайта. Глубина соответствует количеству сегментов урла. Минимальное значение 1. 

- ```-o output_file``` Путь к файлу с результатами.

#### Примеры

###### Пример 1: анализ одной страницы
	> ./CssOptimizer.App.exe http://uawebchallenge.com/

###### Пример 2: анализ нескольких страниц
	> ./CssOptimizer.App.exe http://uawebchallenge.com/ http://devopsreactions.tumblr.com/

###### Пример 3: запись результатов в файл
	> ./CssOptimizer.App.exe http://uawebchallenge.com/ -o result.txt

###### Пример 4: запись результатов в файл в режиме анализа сайта с ограничением количества страниц и глубины
	> ./CssOptimizer.App.exe http://uawebchallenge.com/ -s -c 10 -d 2 -o result.txt

### Известные ограничения
- Не поддерживаются @-правила