using System.Collections;
using UnityEngine;

namespace AbilitySystem.Abilities
{
	[CreateAssetMenu(fileName = "New LightningVolley", menuName = "Abilities/LightningVolley")]
	public class LightningVolley : Ability
	{
		public LightningStrike lightningStrike; // ссылка на LightningStrike
		private AbilityController _controller;
		private GameObject colliderInstance;

		public LightningVolley(RectTransform transform, float effectDuration, float fadeDuration) : base(transform,
			effectDuration, fadeDuration)
		{
			_controller = GameObject.FindObjectOfType<AbilityController>();
		}

		public override void Activate()
		{
			// // Запускаем корутину, которая будет активировать LightningStrike каждые 0.1 секунды
			// _controller.StartCoroutine(ActivateMultipleTimes());
		}

		public override void Activate(Vector3 position, Quaternion rotation)
		{
			_controller = GameObject.FindObjectOfType<AbilityController>();
			colliderInstance = Instantiate(animationPrefab, position, rotation);
			// Запускаем корутину, которая будет активировать LightningStrike каждые 0.1 секунды
			colliderInstance.GetComponent<AbilityAnimationController>().StartCoroutine(ActivateMultipleTimes());
			Destroy(colliderInstance, 2);
		}

		private IEnumerator ActivateMultipleTimes()
		{
			for (int i = 0; i < 15; i++)
			{
				// Генерируем случайную позицию в пределах 25 метров от босса
				Vector3 randomPosition = colliderInstance.transform.position + new Vector3(UnityEngine.Random.Range(-15, 15), 0,
					UnityEngine.Random.Range(-15, 15));

				// Активируем LightningStrike
				_controller.ActivateAbility(lightningStrike, randomPosition, Quaternion.identity);

				// Ждем 0.1 секунды перед следующей активацией
				yield return new WaitForSeconds(0.1f);
			}
		}

		public override void PerformEffect()
		{
			// Не используется в этом классе
		}

		public override IEnumerator FadeOutImages(float currentCastTime)
		{
			// Не используется в этом классе
			yield return null;
		}

		public override IEnumerator DealDamageOverTime()
		{
			// Не используется в этом классе
			yield return null;
		}
	}
}