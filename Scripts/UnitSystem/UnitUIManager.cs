using UnityEngine;

namespace UnitSystem
{
	public class UnitUIManager : MonoBehaviour
	{
		public GameObject unitPrefab; // Префаб юнита
		public Transform allyLayoutGroup; // Layout Group для союзников
		public Transform enemyLayoutGroup; // Layout Group для врагов

		public void AddUnit(Unit unit)
		{
			// Создаем новый экземпляр префаба
			GameObject unitUI = Instantiate(unitPrefab);

			// Получаем компонент UnitUI и устанавливаем юнита
			// Устанавливаем имя и здоровье юнита
			UnitUI unitUIComponent = unitUI.GetComponent<UnitUI>();
			unitUIComponent.SetUnit(unit);
			unitUIComponent.SetName(unit.name);
			unitUIComponent.SetHealth(unit.health);

			// Добавляем юнита в соответствующую группу
			if (unit.isEnemy)
			{
				unitUI.transform.SetParent(enemyLayoutGroup);
			}
			else
			{
				unitUI.transform.SetParent(allyLayoutGroup);
			}

			// Подписываемся на событие изменения здоровья
			unit.onHealthChanged += unitUI.GetComponent<UnitUI>().SetHealth;
		}
	}
}