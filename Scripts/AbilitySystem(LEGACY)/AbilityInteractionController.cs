// using System.Collections;
// using UnityEngine;
//
// namespace AbilitySystem
// {
// 	public class AbilityInteractionController : MonoBehaviour
// 	{
// 		public Ability ability; // ссылка на способность, которую представляет этот объект
//
// 		private void OnTriggerEnter(Collider other)
// 		{
// 			if (other.CompareTag("Player"))
// 			{
// 				ability.playersCollided.Add(other.gameObject);
// 			}
// 		}
//
// 		private void OnTriggerExit(Collider other)
// 		{
// 			if (other.CompareTag("Player"))
// 			{
// 				ability.playersCollided.Remove(other.gameObject);
// 			}
// 		}
// 	}
// }