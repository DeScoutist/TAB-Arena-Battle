using System;
using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Components;
using TMPro;
using UI;
using UnitSystem.UnitAI;
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
		[SerializeField] public AttributeSystem.Authoring.AttributeScriptableObject healthAttribute; // Атрибут здоровья

		[SerializeField]
		private AttributeSystem.Authoring.AttributeScriptableObject maxHealthAttribute; // Атрибут max здоровья

		public Dictionary<Unit, float> threatTable = new Dictionary<Unit, float>(); // Таблица угрозы

		public AbilitySystemCharacter thisAbilitySystemCharacter; // Ссылка на AbilitySystemCharacter

		public float Threat { get; set; }
		public bool isDead = false; // Добавьте эту строку

		public float HealthPercentage
		{
			get
			{
				if (attributeSystem.GetAttributeValue(healthAttribute, out var healthValue))
				{
					attributeSystem.GetAttributeValue(maxHealthAttribute, out var maxHPValue);
					return (healthValue.CurrentValue / maxHPValue.CurrentValue) * 100;
				}

				return 0;
			}
		}

		// Событие, вызываемое при изменении здоровья
		public delegate void OnHealthChanged(float newHealth, Unit dealer, Unit receiver);

		public event OnHealthChanged onHealthChanged;

		// Событие, вызываемое при изменении угрозы
		public delegate void OnThreatChanged(float newThreat, Unit dealer);

		public event OnThreatChanged onThreatChanged;

		private void Start()
		{
			// Получаем компонент AttributeSystemComponent
			attributeSystem = GetComponent<AttributeSystem.Components.AttributeSystemComponent>();
			thisAbilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
		}

		public void AddThreat(Unit dealer, float damage)
		{
			float threatMultiplier = 1f; // Default threat multiplier
			var unitAi = dealer.GetComponent<PlayerAI>();

			// Adjust the threat multiplier based on the type of the dealer
			if (unitAi is DpsAI)
			{
				threatMultiplier = 0.8f;
			}
			else if (unitAi is HealerAI)
			{
				threatMultiplier = 0.6f;
			}
			else if (unitAi is TankAI)
			{
				threatMultiplier = 2.6f;
			}

			float threat = damage * threatMultiplier;

			// If the dealer is already in the threat table, add to their threat
			if (threatTable.ContainsKey(dealer))
			{
				threatTable[dealer] += threat;
			}
			// Otherwise, add the dealer to the threat table
			else
			{
				threatTable.Add(dealer, threat);
			}
			
			onThreatChanged?.Invoke(threat, dealer);
		}

		// Метод для изменения здоровья
		public void ChangeHealth(float amount, Unit dealer, Unit receiver)
		{
			if (thisAbilitySystemCharacter.HasTag(GameplayTag.Undamageable))
			{
				return;
			}

			if (isDead)
			{
				return;
			}

			if (attributeSystem.GetAttributeValue(healthAttribute, out var healthValue))
			{
				// Изменяем базовое значение атрибута здоровья
				float newHealth = healthValue.BaseValue + amount;
				healthValue.BaseValue = Mathf.Max(newHealth, 0);
				attributeSystem.SetAttributeBaseValue(healthAttribute, healthValue.BaseValue);

				if (healthValue.BaseValue <= 0)
				{
					// Debug.Log($"{this.name} dies");
					Die();
				}

				if (amount < 0)
				{
					AddThreat(dealer, -amount);
					ShowDamage(-amount);
				}
				else
				{
					ShowDamage(amount, true);
				}
			}

			attributeSystem.GetAttributeValue(maxHealthAttribute, out var maxHealthValue);
			// Вызываем событие
			onHealthChanged?.Invoke(healthValue.BaseValue / maxHealthValue.BaseValue, dealer, receiver);
		}

		public void ShowDamage(float amount, bool isHeal = false)
		{
			if (isDead)
			{
				return;
			}

			// Получаем 1% от общего здоровья персонажа
			attributeSystem.GetAttributeValue(maxHealthAttribute, out var healthValue);
			float onePercentHealth = healthValue.BaseValue * 0.02f;

			// Если модуль amount меньше 1% от общего здоровья
			if (Math.Abs(amount) < onePercentHealth)
			{
				return;
			}

			// Создайте новый экземпляр префаба урона
			GameObject damageInstance = Instantiate(damagePrefab, transform.position, Quaternion.identity, transform);
			damageInstance.GetComponent<TextMeshPro>().text = amount.ToString();

			// Запустите корутину для анимации текста урона
			StartCoroutine(AnimateDamage(damageInstance, isHeal));
		}

		public void Die()
		{
			// Отключаем все ауры
			var auras = GetComponents<AbilitySystem.Abilities.Auras.IAura>();
			foreach (var aura in auras)
			{
				// Здесь предполагается, что у вас есть метод для отключения ауры.
				// Если такого метода нет, вам нужно будет его добавить.
				aura.Disable();
			}

			if (this.GetComponent<UnitUI>())
			{
				this.GetComponent<UnitUI>().enabled = false;
			}

			// Устанавливаем isDead в true
			isDead = true;
		}

		public void DieInTime(float time)
		{
			StartCoroutine(DieAfterDelay(time));
		}

		private IEnumerator DieAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			Die();
		}

		private IEnumerator AnimateDamage(GameObject damageInstance, bool isHeal = false)
		{
			TextMeshPro damageText = damageInstance.GetComponent<TextMeshPro>();

			float duration = 1f; // Продолжительность анимации
			float scale = 1.5f; // Максимальный масштаб текста
			float elapsed = 0f;

			Vector3 initialScale = damageText.transform.localScale;

			if (isHeal) damageText.color = Color.green;
			else damageText.color = Color.yellow;

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