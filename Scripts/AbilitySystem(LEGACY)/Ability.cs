// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.VFX;
//
// namespace AbilitySystem
// {
// 	public abstract class Ability : ScriptableObject
// 	{
// 		public string abilityName; // имя способности
// 		public float cooldown = 0; // восстановление способности
// 		public float radius = 0; // radius способности
// 		public float damageAmount = 0; // урон, наносимый способностью
// 		public AudioClip abilitySound; // звук, проигрываемый при использовании способности
// 		public float castTime = 0; // время каста способности
// 		public float appearTime = 0; // время активации способности
// 		public float durationTime = 0; // время длительности нахождения способности
// 		public float effectInterval = 0; // время длительности нахождения способности
// 		public GameObject animationPrefab;
// 		public VisualEffect visualEffect;
//
// 		public bool canBeRotatedCasting = true;
// 		public bool isVoidZone = false;
// 		public bool requireCasting = true;
//
// 		public List<GameObject> playersCollided = new List<GameObject>();
//
// 		// метод, в котором описывается логика использования способности
// 		public abstract void Activate();
//
// 		// Добавляем новые параметры в метод Activate
// 		public abstract void Activate(Vector3 position, Quaternion rotation);
//
// 		public RectTransform transform { get; protected set; }
// 		public float effectDuration;
// 		public float fadeDuration;
//
// 		// BOOLS
// 		public bool isAoeShown;
//
// 		protected Ability(RectTransform transform, float effectDuration, float fadeDuration = 0.5f)
// 		{
// 			this.transform = transform;
// 			this.effectDuration = effectDuration;
// 			this.fadeDuration = fadeDuration;
// 		}
//
// 		public abstract void PerformEffect();
//
// 		public abstract IEnumerator FadeOutImages(float currentCastTime);
//
// 		public abstract IEnumerator DealDamageOverTime();
// 	}
// }