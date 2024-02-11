using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnitSystem
{
	public class Unit : MonoBehaviour
	{
		public bool isEnemy; // Является ли юнит врагом
		public bool isCasting = false;
		public bool isTank = false;
		[SerializeField] private GameObject damagePrefab; // Префаб текста урона
		private AttributeSystem.Components.AttributeSystemComponent attributeSystem; // Система атрибутов
		[SerializeField] private AttributeSystem.Authoring.AttributeScriptableObject healthAttribute; // Атрибут здоровья
		[SerializeField] private AttributeSystem.Authoring.AttributeScriptableObject maxHealthAttribute; // Атрибут max здоровья
		public DebuffSystem DebuffSystem { get; private set; }
		
		public float HealthPercentage
		{
			get
			{
				if (attributeSystem.GetAttributeValue(healthAttribute, out var healthValue))
				{
					return (healthValue.CurrentValue / healthValue.BaseValue) * 100;
				}
				return 0;
			}
		}
		
		// Событие, вызываемое при изменении здоровья
		public delegate void OnHealthChanged(float newHealth);
		public event OnHealthChanged onHealthChanged;


		private void Start()
		{
			// Получаем компонент AttributeSystemComponent
			attributeSystem = GetComponent<AttributeSystem.Components.AttributeSystemComponent>();
			// DebuffSystem = new DebuffSystem();
		}

		// Метод для изменения здоровья
		public void ChangeHealth(float amount)
		{
			if (attributeSystem.GetAttributeValue(healthAttribute, out var healthValue))
			{
				// Изменяем базовое значение атрибута здоровья
				float newHealth = healthValue.BaseValue + amount;
				healthValue.BaseValue = Mathf.Max(newHealth, 0);
				attributeSystem.SetAttributeBaseValue(healthAttribute, healthValue.BaseValue);
				
				if (healthValue.BaseValue <= 0)
				{
					Destroy(gameObject);
				}
				
				if (amount < 0)
				{
					ShowDamage(-amount);
				}
			}

			attributeSystem.GetAttributeValue(maxHealthAttribute, out var maxHealthValue);
			// Вызываем событие
			onHealthChanged?.Invoke(healthValue.BaseValue / maxHealthValue.BaseValue);
		}
		
		private void ShowDamage(float amount)
		{
			// Создайте новый экземпляр префаба урона
			GameObject damageInstance = Instantiate(damagePrefab, transform.position, Quaternion.identity, transform);
			damageInstance.GetComponent<TextMeshPro>().text = amount.ToString();

			// Запустите корутину для анимации текста урона
			StartCoroutine(AnimateDamage(damageInstance));
		}

		private IEnumerator AnimateDamage(GameObject damageInstance)
		{
			float duration = 1f; // Продолжительность анимации
			float scale = 1.5f; // Максимальный масштаб текста
			float elapsed = 0f;

			TextMeshPro damageText = damageInstance.GetComponent<TextMeshPro>();
			Vector3 initialScale = damageText.transform.localScale;

			while (elapsed < duration)
			{
				float t = elapsed / duration;

				// Увеличьте и затем уменьшите текст
				float currentScale = Mathf.Lerp(1f, scale, t < 0.5f ? (t * 2f) : (1f - t * 2f));
				damageText.transform.localScale = initialScale * currentScale;

				// Постепенно уменьшите прозрачность текста
				Color color = damageText.color;
				color.a = 1f - t;
				damageText.color = color;

				elapsed += Time.deltaTime;
				yield return null;
			}

			// Удалите экземпляр урона после завершения анимации
			Destroy(damageInstance);
		}
	}
}