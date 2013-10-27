CssOptimizer
------------

### Описание

Консольное приложение для поиска неиспользуемых css-стилей на странице. 

#### Синтаксис 

	> ./CssOptimizer.App.exe <url1> [url2…] [-o output_file]

#### Описание

- url1 [url2…] Адреса анализируемых страниц

- -o Путь к файлу с результатами.

#### Примеры

###### Пример 1: анализ одной страницы
	> ./CssOptimizer.App.exe http://uawebchallenge.com/

###### Пример 2: анализ нескольких страниц
	> ./CssOptimizer.App.exe http://uawebchallenge.com/ http://devopsreactions.tumblr.com/

###### Пример 3: запись результатов в файл
	> ./CssOptimizer.App.exe http://uawebchallenge.com/ -o result.txt

### Возможности

- Анализ всех css-файлов доступных на странице.
	- Css-файлы подключенные через тег ```<link>```
	- Css внутри тега ```<style>```
	
	
- Анализ нескольких страниц

##### В разработке

- Анализ всего сайта с возможностью задания к-ва страниц и глубины вложенности
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

### Известные ограничения
- Не поддерживаются @-правила