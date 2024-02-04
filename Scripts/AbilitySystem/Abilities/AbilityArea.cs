using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Abilities
{
	public class AbilityArea : MonoBehaviour
	{
		// Список игроков в области действия способности
		public List<AbilitySystemCharacter> playersInArea = new List<AbilitySystemCharacter>();

		public float radius = 5f; // Радиус области действия способности
		public LayerMask playerLayer; // Слой, на котором находятся игроки

		// Метод для проверки, находятся ли игроки в области действия способности
		public List<AbilitySystemCharacter> GetPlayersInArea()
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
		
		// private void OnTriggerEnter(Collider other)
		// {
		// 	Debug.Log(other.name);
		// 	// Проверяем, является ли объект игроком
		// 	AbilitySystemCharacter player = other.GetComponent<AbilitySystemCharacter>();
		// 	if (player != null)
		// 	{
		// 		// Добавляем игрока в список
		// 		playersInArea.Add(player);
		// 	}
		// }
		//
		// private void OnTriggerExit(Collider other)
		// {
		// 	// Проверяем, является ли объект игроком
		// 	AbilitySystemCharacter player = other.GetComponent<AbilitySystemCharacter>();
		// 	if (player != null)
		// 	{
		// 		// Удаляем игрока из списка
		// 		playersInArea.Remove(player);
		// 	}
		// }

		// Метод для проверки, находится ли игрок в области действия способности
		public bool IsPlayerInArea(AbilitySystemCharacter player)
		{
			return playersInArea.Contains(player);
		}
	}
}