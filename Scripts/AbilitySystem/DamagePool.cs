﻿using System.Collections;
using System.Collections.Generic;
using UnitSystem;
using UnityEngine;
using UnityEngine.UI;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "New DamagePool", menuName = "Abilities/DamagePool")]
	public class DamagePool : Ability
	{
		private Image explosionZoneInitialBorders;
		private Image explosionZoneProgress;
		private Quaternion currentRotation;

		private GameObject _colliderInstance;

		public DamagePool(RectTransform transform, float effectDuration, float fadeDuration)
			: base(transform, effectDuration, fadeDuration)
		{
		}

		public override void Activate()
		{
		}

		public override void Activate(Vector3 position, Quaternion rotation)
		{
			_colliderInstance = Instantiate(animationPrefab, position, rotation);

			radius *= 0.1f;
			_colliderInstance.transform.localScale = new Vector3(radius, 0, radius);

			_colliderInstance.transform.Find("Fill").GetComponent<AbilityInteractionController>().ability = this;

			// Уничтожить объект через durationTime, а не через fadeDuration
			Destroy(_colliderInstance, fadeDuration);
		}

		public override void PerformEffect()
		{
			// Deal damage to all targets in radius.
			foreach (var target in playersCollided)
			{
				target.GetComponent<Unit>().ChangeHealth(-damageAmount);
			}
		}

		public override IEnumerator FadeOutImages(float currentCastTime)
		{
			var fill = _colliderInstance.transform.Find("Fill");

			for (float t = 0.0f; t < currentCastTime; t += Time.deltaTime)
			{
				var normalizedTime = t / currentCastTime;
				var scaleX = Mathf.Lerp(1, 9.8f, normalizedTime);
				var scaleZ = Mathf.Lerp(1, 9.8f, normalizedTime);
				fill.transform.localScale = new Vector3(scaleX, fill.transform.localScale.y, scaleZ);

				yield return null;
			}
		}

		public override IEnumerator DealDamageOverTime()
		{
			// While the ability is active...
			while (_colliderInstance != null)
			{
				// Wait for a bit before dealing damage again
				yield return new WaitForSeconds(1f);

				// Deal damage to all targets in radius.
				PerformEffect();
			}
		}
	}
}