using System.Collections.Generic;
using AbilitySystem.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class DebuffSystem : MonoBehaviour
	{
		public GameObject DebuffIconPrefab; // Префаб иконки дебаффа
		public Transform DebuffContainer; // Контейнер для иконок дебаффов
		private AbilitySystemCharacter Character; // Ссылка на AbilitySystemCharacter

		private List<GameObject> debuffIcons = new List<GameObject>(); // Список иконок дебаффов

		private void Start()
		{
			Character = this.transform.GetComponent<AbilitySystemCharacter>();
		}

		private void Update()
		{
			// Удаляем старые иконки
			foreach (var icon in debuffIcons)
			{
				Destroy(icon);
			}

			debuffIcons.Clear();

			// Создаем новые иконки для каждого GameplayEffect
			foreach (var gameplayEffect in Character.AppliedGameplayEffects)
			{
				if (gameplayEffect != null && gameplayEffect.spec != null && gameplayEffect.spec.GameplayEffect.Icon != null)
				{
					var debuffIcon = Instantiate(DebuffIconPrefab, DebuffContainer);
					debuffIcons.Add(debuffIcon);

					// Здесь вы можете установить спрайт иконки в соответствии с GameplayEffect
					debuffIcon.transform.Find("Timer").GetComponent<Image>().sprite = gameplayEffect.spec.GameplayEffect.Icon;

					// Обновляем полоску времени действия и текст таймера
					var timerImage = debuffIcon.transform.Find("Timer").GetComponent<Image>();
					var timerText = debuffIcon.transform.Find("Timer/TimerText").GetComponent<TMP_Text>();
				
					if (timerImage != null && timerText != null)
					{
						timerImage.fillAmount = gameplayEffect.spec.DurationRemaining / gameplayEffect.spec.TotalDuration;
						timerText.text = gameplayEffect.spec.DurationRemaining.ToString("F1");

						if (gameplayEffect.spec.DurationRemaining > 3)
						{
							timerText.color = Color.white;
						}
						else
						{
							timerText.color = Color.red;
						}
					}
				}
			}
		}
	}
}