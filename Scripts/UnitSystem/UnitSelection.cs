using UnitSystem;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
	public static Unit selectedUnit; // Выделенный юнит

	void Update()
	{
		// Проверяем, был ли клик мыши
		if (Input.GetMouseButtonDown(0))
		{
			// Создаем луч из камеры в точку клика
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			// Проверяем, попал ли луч в какой-нибудь объект
			if (Physics.Raycast(ray, out hit))
			{
				// Проверяем, является ли объект юнитом
				Unit unit = hit.transform.GetComponent<Unit>();
				if (unit != null && !unit.isEnemy)
				{
					// Выделяем юнит
					SelectUnit(unit);
				}
			}
			else
			{
				DeselectUnit();
			}
		}
	}

	public static void SelectUnit(Unit unit)
	{
		// Если уже есть выделенный юнит, снимаем с него выделение
		if (selectedUnit != null)
		{
			DeselectUnit();
		}

		// Выделяем новый юнит
		selectedUnit = unit;

		// Меняем визуальное представление юнита
		// Например, вы можете изменить цвет юнита или добавить вокруг него выделение
		// Это зависит от вашей реализации
		selectedUnit.GetComponentInChildren<SpriteRenderer>().material.color = Color.red;
	}

	public static void DeselectUnit()
	{
		// Возвращаем юниту исходный цвет
		selectedUnit.GetComponentInChildren<SpriteRenderer>().material.color = Color.white;

		// Снимаем выделение с юнита
		selectedUnit = null;
	}
}