using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnitSystem
{
	public class UnitUI : MonoBehaviour, IPointerClickHandler
	{
		public TextMeshProUGUI nameText; // UI элемент для отображения имени
		public Image healthSlider; // UI элемент для отображения здоровья
		private Unit _unit; // Ссылка на юнит

		public void SetUnit(Unit unit)
		{
			this._unit = unit;

			// Подписываемся на событие изменения здоровья
			unit.onHealthChanged += SetHealth;

			// Устанавливаем начальные значения
			SetName(unit.name);
			SetHealth(unit.health);
		}

		public void SetName(string name)
		{
			// Устанавливаем текст имени
			nameText.text = name;
		}

		public void SetHealth(float health)
		{
			// Устанавливаем значение слайдера здоровья
			healthSlider.fillAmount = health;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			// Вызываем метод выделения юнита при клике на элемент интерфейса
			SelectUnit();
		}

		private void SelectUnit()
		{
			// Вызываем метод выделения юнита из класса UnitSelection
			UnitSelection.SelectUnit(_unit);
		}
	}
}