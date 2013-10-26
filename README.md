CssOptimizer
------------

### Описание

Консольное приложение для поиска неиспользуемых css-стилей на странице. 

### Использование 

	> ./CssOptimizer.App.exe http://uawebchallenge.com/ http://google.com.ua
	
### Возможноси

- Анализ всех css-файлов доступных на странице.
	- Css-файлы подключенные через тег ```<link>```
	- Css внутри тега ```<style>```
	- Css-файлы подключенные через правило ```@import```
	
- Анализ нескольких страниц

##### В разработке

- Анализ всего сайта с возможностью задания к-ва страниц и глубины вложенности

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
- Не поддерживаются все @-правила, кроме ```@import```