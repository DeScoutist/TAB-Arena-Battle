// Aura.cs

using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Authoring;
using UnityEngine;

namespace AbilitySystem.Abilities.Auras
{
	public class PlagueCloudAura : MonoBehaviour, IAura
	{
		[SerializeField] private AuraTargets targets;
		[SerializeField] private GameplayEffectScriptableObject effect;
		[SerializeField] private float radius;

		private List<AbilitySystemCharacter> playersInRadius = new List<AbilitySystemCharacter>();

		public AuraTargets Targets
		{
			get { return targets; }
			set { targets = value; }
		}

		public GameplayEffectScriptableObject Effect
		{
			get { return effect; }
			set { effect = value; }
		}

		public float Radius
		{
			get { return radius; }
			set { radius = value; }
		}

		public void Update()
		{
			// Получаем всех игроков в радиусе
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
			foreach (var hitCollider in hitColliders)
			{
				var player = hitCollider.GetComponent<AbilitySystemCharacter>();
				if (player != null)
				{
					// Если игрок в радиусе, наносим ему урон каждые три секунды
					if (!playersInRadius.Contains(player))
					{
						playersInRadius.Add(player);
					}

					ApplyEffect(player);
				}
			}

			// Если игрок вышел из радиуса, начинаем отсчет времени
			for (int i = playersInRadius.Count - 1; i >= 0; i--)
			{
				if (!hitColliders.Contains(playersInRadius[i].GetComponent<Collider>()))
				{
					playersInRadius.RemoveAt(i);
				}
			}
		}

		private void ApplyEffect(AbilitySystemCharacter player)
		{
			// Создаем GameplayEffectSpec из GameplayEffectScriptableObject
			var effectSpec = player.MakeOutgoingSpec(Effect);

			// Применяем основной эффект способности к игроку
			player.ApplyGameplayEffectSpecToSelf(effectSpec);

			// Применяем условные эффекты, если они есть
			foreach (var conditionalEffect in Effect.gameplayEffect.ConditionalGameplayEffects)
			{
				// Проверяем, есть ли у игрока все необходимые теги
				bool hasAllRequiredTags = true;
				foreach (var requiredTag in conditionalEffect.RequiredSourceTags)
				{
					if (!player.AppliedGameplayEffects.Any(effect =>
						    effect.spec.GameplayEffect.gameplayEffectTags.GrantedTags.Contains(requiredTag)))
					{
						hasAllRequiredTags = false;
						break;
					}
				}

				// Если у игрока есть все необходимые теги, применяем ConditionalGameplayEffect
				if (hasAllRequiredTags)
				{
					var conditionalEffectSpec = player.MakeOutgoingSpec(conditionalEffect.GameplayEffect);
					player.ApplyGameplayEffectSpecToSelf(conditionalEffectSpec);
				}
			}
		}
	}
}