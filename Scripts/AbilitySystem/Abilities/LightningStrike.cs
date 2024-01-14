using System.Collections;
using UnityEngine;

namespace AbilitySystem.Abilities
{
	[CreateAssetMenu(fileName = "New Lightning", menuName = "Abilities/ExplosionZone/LightningStrike")]
	public class LightningStrike : ExplosionZone
	{
		public GameObject firstEffectPrefab; // Первый эффект
		public GameObject secondEffectPrefab; // Второй эффект
		public GameObject thirdEffectPrefab; // Третий эффект

		public LightningStrike(RectTransform transform, float effectDuration, float fadeDuration) : base(transform, effectDuration, fadeDuration)
		{
		}

		public override void Activate(Vector3 position, Quaternion rotation)
		{
			colliderInstance = Instantiate(animationPrefab, position, rotation);
			// Создаем первый эффект
			GameObject firstEffectInstance = Instantiate(firstEffectPrefab, position, rotation);
			Destroy(firstEffectInstance, appearTime); // Уничтожаем первый эффект через 3 секунды

			// Запускаем корутину, которая будет ждать 3 секунды, затем создаст второй и третий эффекты
			colliderInstance.GetComponent<AbilityAnimationController>().StartCoroutine(PerformEffects(position, rotation));
			
			Destroy(colliderInstance, appearTime + durationTime + fadeDuration + 1);
		}

		public IEnumerator PerformEffects(Vector3 position, Quaternion rotation)
		{
			// Ждем 3 секунды
			yield return new WaitForSeconds(appearTime);

			// Создаем второй эффект
			GameObject secondEffectInstance = Instantiate(secondEffectPrefab, position, rotation);
			Destroy(secondEffectInstance, durationTime); // Уничтожаем второй эффект через 1 секунду

			// Выполняем проверку на столкновение с игроками
			PerformEffect();

			// Создаем третий эффект
			GameObject thirdEffectInstance = Instantiate(thirdEffectPrefab, position, rotation);
			Destroy(thirdEffectInstance, fadeDuration); // Уничтожаем третий эффект через 5 секунд
		}
	}
}