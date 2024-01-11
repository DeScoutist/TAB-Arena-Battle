using System;
using UnityEngine;

namespace UnitSystem
{
	public class Unit : MonoBehaviour
	{
		public string name; // Имя юнита
		public float health; // Здоровье юнита
		public float maxHealth; // Здоровье юнита
		public bool isEnemy; // Является ли юнит врагом
		public bool isCasting = false;

		// Событие, вызываемое при изменении здоровья
		public delegate void OnHealthChanged(float newHealth);
		public event OnHealthChanged onHealthChanged;

		private void Start()
		{
			maxHealth = health;
		}

		// Метод для изменения здоровья
		public void ChangeHealth(float amount)
		{
			health += amount;

			// Ограничиваем здоровье между 0 и 100
			health = Mathf.Clamp(health, 0, 100);

			Debug.Log(amount);
			Debug.Log(health);
			// Вызываем событие
			onHealthChanged?.Invoke(health / maxHealth);
		}
	}
}