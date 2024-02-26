using TMPro;
using UnityEngine;

namespace UnitSystem
{
	public class DebugLog : MonoBehaviour
	{
		private TMP_Text debugText;

		private void Start()
		{
			// Получаем ссылку на текстовый компонент Debug
			debugText = GameObject.Find("Debug").GetComponent<TMP_Text>();

			// Подписываемся на событие изменения выбранного юнита
			UnitSelection.onSelectedUnitChanged += OnSelectedUnitChanged;
		}

		private void OnSelectedUnitChanged(Unit newSelectedUnit)
		{
			// Если был выбран новый юнит
			if (newSelectedUnit != null)
			{
				// Подписываемся на событие изменения угрозы нового выбранного юнита
				newSelectedUnit.onThreatChanged += UpdateDebugText;
			}
		}

		private void UpdateDebugText(float newThreat, Unit dealer)
		{
			// Обновляем текстовый компонент Debug
			debugText.text = $"{dealer.name} Threat List:\n";

			foreach (var threat in dealer.threatTable)
			{
				debugText.text += $"Unit: {threat.Key.name}, Threat: {threat.Value}\n";
			}
		}
	}
}