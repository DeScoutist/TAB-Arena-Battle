Система `SpawnUnit` привязана к `Canvas`. В нём указаны все префабы юнитов и нулевая координата.

## Unit (Prefab)

`Unit (Prefab)` обладает следующими привязанностями:
- `Unit`
- `Rigidbody`
- `UnitSelection`
- `AbilityInteractionController`
- `PlayerAI`

Также внутри префаба вложен объект `Icon`, в котором есть `SpriteRenderer`.


Как добавить нового персонажа:

1) Создать префаб персонажа (КУБ например);
2) Добавить ему следующие компоненты:
- Unit;
- AttributeSystemComponent;
- AbilityController;
- UnitUI;
- DebuffSystem;
- Vision;
2.1) Если это персонаж игрока - нужно добавить PlayerAI;
2.2) Если это персонаж босс или противник - BasicEnemyAI;

Как добавить новый стат:

1) Assets -> Create -> Gameplay Ability System -> Attribute;
2) (Добавить зависящие от него параметры, например "Сила даёт Реген") Assets -> Create -> Gameplay Ability System -> Linear Derived Attribute;
3) (Добавить сам доп. аттрибут) Assets -> Create -> Gameplay Ability System -> Gameplay Effect -> Modifier Magnitude -> Attribute Backed Modifier;
4) (Добавить эффект этого аттрибута на персонажа) Assets -> Create -> Gameplay Ability System -> Gameplay Effect Definition;

## Создание способности
Как добавить новый скилл:

1) Gameplay Tag - для Стоимости, Кулдауна и самого эффекта.
2) Gameplay Effect Definiton - для Стоимости, Кулдауна и самого эффекта.
2.1) Duration:
- Instant (на один кадр);
- Infinite (навсегда);
- Has Duration (на время);

В Модификатор грузим Linear Simple Float Modifier Magnitude (1), мультипликатор длительности - определяет сколько секунд;
2.2) Modifiers: 
- Добавляем те аттрибуты, которые должны изменяться;
- Добавляем какой характер изменений (Add - добавить (можно с минусом), Multiply - умножить (можно на дробь), Override - перезаписать);
- Магнитуда (Linear Simple Float Modifier Magnitude (1));
- Мультипликатор - количество изменяемого аттрибута.

2.3) Conditional Gameplay Effects - какие еще эффекты навесит способность;
2.3.1) Required Source Tags - Тег который ДОЛЖЕН ПРИСУТСТВОВАТЬ на цели чтобы способность СРАБОТАЛА.

2.4) Asset Tag - Тег который способность навесит на юзающего;

2.5) Require Tag - тег который ДОЛЖЕН висеть чтобы СРАБОТАЛО;
2.6) Ignore Tags - тег которого НЕ ДОЛЖНО БЫТЬ чтобы СРАБОТАЛО;

2.7) Remove Gameplay Effects with Tag - убирает с цели эффекты с тегами из перечисления;

2.8) Period - Сколько секунд должно пройти до следующего тика эффекта;
2.9) Execute on Application - Сработка при нанесении эффекта;

2.10) Max Stacks - максимальное число стаков эффекта;
2.11) Stacks Renewing - При наложении нового стака происходит обновление длительности ВСЕХ стаков;
2.12) Refreshing Tick Period - При наложении нового эффекта время до срабатывания обнуляется(было 2 секунды из 3 до сработки, наложили новый стак - снова 3 секунды до сработки).

3.1) Используем Assets -> Create -> Gameplay Ability System -> Abilities -> Ваша способность (или выбери из списка доступных);
3.2) Если нет подходящего скилла - создаём новый скрипт способности. Копируем самый похожий по функционалу и редактируем методы Activate, PreActivate. 
3.3) СШИВАЕМ наши эффекты в нашу способность (эффект Кулдауна, Цены и самой способности).
3.4) ...
3.5) PROFIT!



