using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using AbilitySystem.Components;
using UnityEngine;

// Ensure to include the namespace where AuraAbilityScriptableObject is defined

namespace UnitSystem
{
	public class CharacterAuraSystem : MonoBehaviour
	{
		// Список игроков в области действия способности
		public List<AbilitySystemCharacter> playersInArea = new List<AbilitySystemCharacter>();

		public LayerMask playerLayer; // Слой, на котором находятся игроки

		// List to store the auras
		public List<AuraAbilityScriptableObject> auras = new List<AuraAbilityScriptableObject>();

		private void Update()
		{
			// Apply all abilities from the auras
			foreach (var aura in auras)
			{
				ApplyAuraAbility(aura);
			}
		}

		// Method to apply an aura ability
		private void ApplyAuraAbility(AuraAbilityScriptableObject aura)
		{
			// Get the players in the area of the aura
			var playersInArea = GetPlayersInArea(aura.radius);

			// Apply the aura ability to each player in the area
			foreach (var player in playersInArea)
			{
				// Here you can call the method to apply the ability to the player
				// The implementation of this method will depend on your game logic
			}
		}

		// Метод для проверки, находятся ли игроки в области действия способности
		public List<AbilitySystemCharacter> GetPlayersInArea(float radius)
		{
			List<AbilitySystemCharacter> playersInArea = new List<AbilitySystemCharacter>();

			// Получаем все коллайдеры в радиусе действия способности
			Collider[] colliders = Physics.OverlapSphere(transform.position, radius, playerLayer);

			// Проверяем каждый коллайдер
			foreach (Collider collider in colliders)
			{
				// Если коллайдер принадлежит игроку, добавляем его в список
				AbilitySystemCharacter player = collider.GetComponent<AbilitySystemCharacter>();
				if (player != null)
				{
					playersInArea.Add(player);
				}
			}

			return playersInArea;
		}

		// Метод для проверки, находится ли игрок в области действия способности
		public bool IsPlayerInArea(AbilitySystemCharacter player)
		{
			return playersInArea.Contains(player);
		}
	}
}