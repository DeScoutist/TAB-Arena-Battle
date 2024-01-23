// using System.Collections;
// using UnityEngine;
//
// namespace AbilitySystem
// {
// 	[CreateAssetMenu(fileName = "New Fireball", menuName = "Abilities/Fireball")]
// 	public class Fireball : Ability
// 	{
// 		public GameObject fireballPrefab; // Prefab for the fireball
// 		public bool penetratePlayers; // Whether the fireball should penetrate players
//
// 		public Fireball(RectTransform transform, float effectDuration, float fadeDuration)
// 			: base(transform, effectDuration, fadeDuration)
// 		{
// 		}
//
// 		public override void Activate()
// 		{
// 		}
//
// 		public override void Activate(Vector3 position, Quaternion rotation)
// 		{
// 			// Instantiate the fireball and set its direction
// 			GameObject fireballInstance = Instantiate(fireballPrefab, position, rotation);
// 			FireballController fireballController = fireballInstance.GetComponent<FireballController>();
// 			fireballController.damageAmount = damageAmount;
// 			fireballController.penetratePlayers = penetratePlayers;
// 		}
//
// 		public override void PerformEffect()
// 		{
// 			// The effect is performed in the FireballController script attached to the fireball prefab
// 			return;
// 		}
//
// 		public override IEnumerator FadeOutImages(float time)
// 		{
// 			return null;
// 		}
//
// 		public override IEnumerator DealDamageOverTime()
// 		{
// 			yield return null;
// 		}
// 	}
// }