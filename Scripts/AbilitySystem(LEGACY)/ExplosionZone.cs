// ﻿using System.Collections;
// using UnitSystem;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace AbilitySystem
// {
// 	[CreateAssetMenu(fileName = "New ExplosionZone", menuName = "Abilities/ExplosionZone")]
// 	public class ExplosionZone : Ability
// 	{
// 		private Image explosionZoneInitialBorders;
// 		private Image explosionZoneProgress;
// 		private Quaternion currentRotation;
//
// 		protected GameObject colliderInstance;
//
// 		public ExplosionZone(RectTransform transform, float effectDuration, float fadeDuration)
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
// 			colliderInstance = Instantiate(animationPrefab, position, rotation);
//
// 			radius *= 0.1f;
// 			colliderInstance.transform.localScale = new Vector3(radius, 0, radius);
// 			colliderInstance.GetComponent<AbilityAnimationController>().StartCoroutine(FadeOutImages(appearTime));
//
// 			colliderInstance.transform.Find("Fill").GetComponent<AbilityInteractionController>().ability = this;
//
// 			// Уничтожить объект через 5 секунд, например
// 			Destroy(colliderInstance, appearTime);
// 		}
//
// 		public override void PerformEffect()
// 		{
// 			// Once the effect has ended...
// 			// Deal damage to all targets in radius.
// 			foreach (var target in playersCollided)
// 			{
// 				target.GetComponent<Unit>().ChangeHealth(-damageAmount);
// 			}
// 		}
//
// 		public override IEnumerator FadeOutImages(float currentCastTime)
// 		{
// 			var fill = colliderInstance.transform.Find("Fill");
//
// 			for (float t = 0.0f; t < currentCastTime; t += Time.deltaTime)
// 			{
// 				var normalizedTime = t / currentCastTime;
// 				// var normalizedTime = 0.1f;
// 				// var alpha = Mathf.Lerp(0, 0.7f, normalizedTime);
// 				// var positionZ = Mathf.Lerp(5, 0, normalizedTime);
// 				// var scaleZ = Mathf.Lerp(0, 1, normalizedTime);
// 				var scaleX = Mathf.Lerp(1, 9.8f, normalizedTime);
// 				var scaleZ = Mathf.Lerp(1, 9.8f, normalizedTime);
// 				// var scaleX = Mathf.Lerp(0, 1, normalizedTime);
// 				// var offsetY = Mathf.Lerp(-0.5f, 0.5f, normalizedTime);
// 				// var tilingX = Mathf.Lerp(3, 1, normalizedTime);
// 				// var offsetX = Mathf.Lerp(-1, 0, normalizedTime);
// 				// var fill = _colliderInstance.transform.Find("Fill");
// 				fill.transform.localScale = new Vector3(scaleX, fill.transform.localScale.y, scaleZ);
// 				// fill.transform.localScale = new Vector3(scaleX, fill.transform.localScale.y, fill.transform.localScale.z);
// 				// fill.GetComponent<Renderer>().material.mainTextureScale = new Vector2(tilingX, 1);
// 				// fill.GetComponent<Renderer>().material.SetTextureOffset("_DetailAlbedoMap", new Vector2(0, offsetY));
// 				// fill.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0, offsetY);
// 				// fill.GetComponent<Renderer>().material.color = new Color(0.8f, 0, 0, alpha);
//
// 				yield return null;
// 			}
//
// 			PerformEffect();
// 		}
//
// 		public override IEnumerator DealDamageOverTime()
// 		{
// 			yield return null;
// 		}
// 	}
// }