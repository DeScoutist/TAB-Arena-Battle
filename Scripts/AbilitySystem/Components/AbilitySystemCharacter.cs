using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTags.Authoring;
using UnitSystem;
using UnityEngine;

namespace AbilitySystem.Components
{
	public class AbilitySystemCharacter : MonoBehaviour
	{
		[SerializeField] protected AttributeSystemComponent _attributeSystem;

		// Определите делегат, который представляет сигнатуру методов, которые могут быть подписаны на событие
		public delegate void GameplayEffectChanged(GameplayEffectScriptableObject gameplayEffect);

		public AttributeSystemComponent AttributeSystem
		{
			get { return _attributeSystem; }
			set { _attributeSystem = value; }
		}

		public List<GameplayEffectContainer> AppliedGameplayEffects = new List<GameplayEffectContainer>();
		public List<AbstractAbilitySpec> GrantedAbilities = new List<AbstractAbilitySpec>();
		public float Level;

		public void GrantAbility(AbstractAbilitySpec spec)
		{
			this.GrantedAbilities.Add(spec);
		}

		public void DebugPrintGrantedTags()
		{
			// foreach (var gameplayEffect in AppliedGameplayEffects)
			// {
			// 	foreach (var tag in gameplayEffect.spec.GameplayEffect.gameplayEffectTags.GrantedTags)
			// 	{
			// 		Debug.Log(tag.ToString());
			// 	}
			// }
		}

		// public void AuraProcs()
		// {
		// 	foreach (AbstractAbilitySpec ability in GrantedAbilities)
		// 	{
		// 		if (ability is AuraAbilityScriptableObject.AuraAbilitySpec auraAbility)
		// 		{
		// 			auraAbility.Update();
		// 		}
		// 	}
		// }

		public void RemoveAbilitiesWithTag(GameplayTagScriptableObject tag)
		{
			for (var i = GrantedAbilities.Count - 1; i >= 0; i--)
			{
				if (GrantedAbilities[i].Ability.AbilityTags.AssetTag == tag)
				{
					GrantedAbilities.RemoveAt(i);
				}
			}
		}


		/// <summary>
		/// Applies the gameplay effect spec to self
		/// </summary>
		/// <param name="geSpec">GameplayEffectSpec to apply</param>
		public bool ApplyGameplayEffectSpecToSelf(GameplayEffectSpec geSpec)
		{
			if (geSpec == null) return true;
			bool tagRequirementsOK = CheckTagRequirementsMet(geSpec);

			if (tagRequirementsOK == false) return false;

			// Debug.Log("Applying GameplayEffect: " + geSpec.GameplayEffect.name); // Добавьте эту строку

			switch (geSpec.GameplayEffect.gameplayEffect.DurationPolicy)
			{
				case EDurationPolicy.HasDuration:
				case EDurationPolicy.Infinite:
					ApplyDurationalGameplayEffect(geSpec);
					break;
				case EDurationPolicy.Instant:
					ApplyInstantGameplayEffect(geSpec);
					return true;
			}

			return true;
		}

		public GameplayEffectSpec MakeOutgoingSpec(GameplayEffectScriptableObject GameplayEffect, float? level = 1f)
		{
			level = level ?? this.Level;
			return GameplayEffectSpec.CreateNew(
				GameplayEffect: GameplayEffect,
				Source: this,
				Level: level.GetValueOrDefault(1));
		}

		bool CheckTagRequirementsMet(GameplayEffectSpec geSpec)
		{
			/// Build temporary list of all gametags currently applied
			var appliedTags = new List<GameplayTagScriptableObject>();
			for (var i = 0; i < AppliedGameplayEffects.Count; i++)
			{
				appliedTags.AddRange(AppliedGameplayEffects[i].spec.GameplayEffect.gameplayEffectTags.GrantedTags);
			}

			// Every tag in the ApplicationTagRequirements.RequireTags needs to be in the character tags list
			// In other words, if any tag in ApplicationTagRequirements.RequireTags is not present, requirement is not met
			for (var i = 0;
			     i < geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.RequireTags.Length;
			     i++)
			{
				if (!appliedTags.Contains(geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements
					    .RequireTags[i]))
				{
					return false;
				}
			}

			// No tag in the ApplicationTagRequirements.IgnoreTags must in the character tags list
			// In other words, if any tag in ApplicationTagRequirements.IgnoreTags is present, requirement is not met
			for (var i = 0;
			     i < geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags.Length;
			     i++)
			{
				if (appliedTags.Contains(
					    geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags[i]))
				{
					return false;
				}
			}

			return true;
		}

		void ApplyInstantGameplayEffect(GameplayEffectSpec spec)
		{
			Unit dealer = spec.Source.GetComponent<Unit>(); // The unit who applied the effect
			// Debug.Log($"{dealer.name} + {spec.GameplayEffect.name}");
			Unit receiver = this.GetComponent<Unit>(); // The unit who is affected

			for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
			{
				var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
				var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier)
					.GetValueOrDefault();

				// Найти количество стаков для этого эффекта
				var effectContainer =
					AppliedGameplayEffects.FirstOrDefault(e => e.spec.GameplayEffect == spec.GameplayEffect);
				var stacks = effectContainer != null ? effectContainer.Stacks : 1;

				// Умножить величину на количество стаков
				magnitude *= stacks;

				var attribute = modifier.Attribute;
				this.AttributeSystem.GetAttributeValue(attribute, out var attributeValue);

				if (attribute == this.GetComponent<Unit>().healthAttribute)
				{
					if (!this.HasTag(GameplayTag.Undamageable))
					{
						switch (modifier.ModifierOperator)
						{
							case EAttributeModifier.Add:
								attributeValue.BaseValue += magnitude;
								if (magnitude < 0)
								{
									this.GetComponent<Unit>().ChangeHealth(-magnitude, dealer, receiver);
								}
								else
								{
									this.GetComponent<Unit>().ChangeHealth(magnitude, dealer, receiver);
								}

								break;
							case EAttributeModifier.Multiply:
								attributeValue.BaseValue *= magnitude;
								break;
							case EAttributeModifier.Override:
								attributeValue.BaseValue = magnitude;
								break;
						}
					}
				}
				else
				{
					switch (modifier.ModifierOperator)
					{
						case EAttributeModifier.Add:
							attributeValue.BaseValue += magnitude;
							break;
						case EAttributeModifier.Multiply:
							attributeValue.BaseValue *= magnitude;
							break;
						case EAttributeModifier.Override:
							attributeValue.BaseValue = magnitude;
							break;
					}
				}


				this.AttributeSystem.SetAttributeBaseValue(attribute, attributeValue.BaseValue);
			}
		}

		void ApplyDurationalGameplayEffect(GameplayEffectSpec spec)
		{
			var modifiersToApply = new List<GameplayEffectContainer.ModifierContainer>();
			for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
			{
				var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
				var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier)
					.GetValueOrDefault();
				var attributeModifier = new AttributeModifier();
				switch (modifier.ModifierOperator)
				{
					case EAttributeModifier.Add:
						attributeModifier.Add = magnitude;
						break;
					case EAttributeModifier.Multiply:
						attributeModifier.Multiply = magnitude;
						break;
					case EAttributeModifier.Override:
						attributeModifier.Override = magnitude;
						break;
				}

				modifiersToApply.Add(new GameplayEffectContainer.ModifierContainer()
					{ Attribute = modifier.Attribute, Modifier = attributeModifier });
			}

			// Debug.Log($"{spec.GameplayEffect.name}");

			foreach (var effect in AppliedGameplayEffects)
			{
				// Debug.Log($"{effect.spec.GameplayEffect.name} + {this.gameObject.name}");
			}

			// Debug.Log($"{spec.GameplayEffect.name}");
			// Проверяем, есть ли уже такой эффект на персонаже
			var existingEffect = AppliedGameplayEffects.FirstOrDefault(effectContainer =>
				effectContainer.spec.GameplayEffect == spec.GameplayEffect);
			// Debug.Log($"Checking for existing effect: {spec.GameplayEffect.name}");

			if (existingEffect != null)
			{
				// Debug.Log($"Existing effect found: {existingEffect.spec.GameplayEffect.name}");
				if (existingEffect.spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Infinite)
				{
					// Debug.Log($"Duration policy is infinite. Adding new effect and returning. {existingEffect.spec.GameplayEffect.name}");
					AppliedGameplayEffects.Add(new GameplayEffectContainer()
						{ spec = spec, modifiers = modifiersToApply.ToArray() });
					return;
				}

				if (existingEffect.Stacks < spec.GameplayEffect.MaxStacks)
				{
					// Debug.Log($"Stacks not maxed. Incrementing stacks. {existingEffect.spec.GameplayEffect.name}");
					existingEffect.Stacks++;
				}

				if (spec.GameplayEffect.StacksRenewing)
				{
					// Debug.Log($"StacksRenewing is true. Updating duration and possibly TimeUntilPeriodTick. {existingEffect.spec.GameplayEffect.name}");
					existingEffect.spec.DurationRemaining = spec.GameplayEffect.gameplayEffect.DurationMultiplier;
					if (spec.GameplayEffect.RefreshingTickPeriod)
					{
						existingEffect.spec.TimeUntilPeriodTick = spec.GameplayEffect.Period.Period;
					}
				}
				else
				{
					// Debug.Log($"StacksRenewing is false. Looking for oldest effect to replace. {existingEffect.spec.GameplayEffect.name}");
					var oldestEffect = AppliedGameplayEffects
						.Where(effectContainer =>
							effectContainer.spec.GameplayEffect.gameplayEffectTags.AssetTag.Equals(spec.GameplayEffect
								.gameplayEffectTags.AssetTag))
						.OrderBy(effectContainer => effectContainer.spec.DurationRemaining)
						.FirstOrDefault();

					if (oldestEffect != null)
					{
						// Debug.Log($"Oldest effect found: {oldestEffect.spec.GameplayEffect.name}. Removing it.");
						AppliedGameplayEffects.Remove(oldestEffect);
					}

					// Debug.Log($"Adding new effect. {existingEffect.spec.GameplayEffect.name}");
					AppliedGameplayEffects.Add(new GameplayEffectContainer()
						{ spec = spec, modifiers = modifiersToApply.ToArray() });
				}
			}
			else
			{
				// Debug.Log($"No existing effect found. Adding new effect.");
				AppliedGameplayEffects.Add(new GameplayEffectContainer()
					{ spec = spec, modifiers = modifiersToApply.ToArray() });
			}
		}

		void UpdateAttributeSystem()
		{
			// Set Current Value to Base Value (default position if there are no GE affecting that atribute)


			for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
			{
				var modifiers = this.AppliedGameplayEffects[i].modifiers;
				for (var m = 0; m < modifiers.Length; m++)
				{
					var modifier = modifiers[m];
					AttributeSystem.UpdateAttributeModifiers(modifier.Attribute, modifier.Modifier, out _);
				}
			}
		}

		void TickGameplayEffects()
		{
			for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
			{
				var ge = this.AppliedGameplayEffects[i].spec;

				// Can't tick instant GE
				if (ge.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Instant) continue;

				// Update time remaining.  Stritly, it's only really valid for durational GE, but calculating for infinite GE isn't harmful
				ge.UpdateRemainingDuration(Time.deltaTime);

				// Tick the periodic component
				ge.TickPeriodic(Time.deltaTime, out var executePeriodicTick);
				if (executePeriodicTick)
				{
					ApplyInstantGameplayEffect(ge);
				}

				if (ge.DurationRemaining <= 0 &&
				    ge.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration)
				{
					AppliedGameplayEffects.RemoveAt(i);
				}
			}
		}

		void CleanGameplayEffects()
		{
			this.AppliedGameplayEffects.RemoveAll(x =>
				x.spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration &&
				x.spec.DurationRemaining <= 0);

			// for (int i = AppliedGameplayEffects.Count - 1; i >= 0; i--)
			// {
			// 	var gameplayEffectContainer = AppliedGameplayEffects[i];
			// 	if (gameplayEffectContainer.spec.GameplayEffect.gameplayEffect.DurationPolicy ==
			// 	    EDurationPolicy.HasDuration
			// 	    && gameplayEffectContainer.spec.DurationRemaining <= 0)
			// 	{
			// 		// Удалите GrantedTags здесь
			// 		foreach (var tag in gameplayEffectContainer.spec.GameplayEffect.gameplayEffectTags.GrantedTags)
			// 		{
			// 			// Создайте новый список тегов, исключив тег, который вы хотите удалить
			// 			var newTagsList = new List<GameplayTagScriptableObject>(gameplayEffectContainer.spec
			// 				.GameplayEffect.gameplayEffectTags.GrantedTags);
			// 			newTagsList.Remove(tag);
			//
			// 			// Присвойте новый список обратно в GrantedTags
			// 			gameplayEffectContainer.spec.GameplayEffect.gameplayEffectTags.GrantedTags =
			// 				newTagsList.ToArray();
			// 		}
			// 		
			// 		AppliedGameplayEffects.RemoveAt(i);
			// 	}
			// }
		}

		void Update()
		{
			// Reset all attributes to 0
			this.AttributeSystem.ResetAttributeModifiers();
			UpdateAttributeSystem();

			// AuraProcs();
			// DebugPrintGrantedTags();
			TickGameplayEffects();
			CleanGameplayEffects();
		}

		public bool HasTag(GameplayTag tag)
		{
			string tagName = tag.ToString();
			foreach (var gameplayEffect in AppliedGameplayEffects)
			{
				foreach (var grantedTag in gameplayEffect.spec.GameplayEffect.gameplayEffectTags.GrantedTags)
				{
					Debug.Log($"{this.gameObject.name} have tag: {grantedTag.name}, we are comparing it to {tag.ToString()}");
					if (grantedTag.name == tagName)
					{
						Debug.Log("Has tag: " + tagName);
						return true;
					}
				}
			}

			return false;
		}
	}


	public class GameplayEffectContainer
	{
		public GameplayEffectSpec spec;
		public ModifierContainer[] modifiers;
		public int Stacks = 1;

		public class ModifierContainer
		{
			public AttributeScriptableObject Attribute;
			public AttributeModifier Modifier;
		}
	}

	public enum GameplayTag
	{
		Stunned,
		Undamageable,
	}
}