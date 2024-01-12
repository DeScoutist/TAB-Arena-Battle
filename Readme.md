# SpawnUnit System

Система `SpawnUnit` привязана к `Canvas`. В нём указаны все префабы юнитов и нулевая координата.

## Unit (Prefab)

`Unit (Prefab)` обладает следующими привязанностями:
- `Unit`
- `Rigidbody`
- `UnitSelection`
- `AbilityInteractionController`
- `PlayerAI`

Также внутри префаба вложен объект `Icon`, в котором есть `SpriteRenderer`.

## Создание способности

Чтобы создать способность нужно:
- Создать `ScriptableObject` способности.