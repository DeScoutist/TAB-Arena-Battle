using System.Collections;
using AbilitySystem.Components;
using UnitSystem.UnitAI;
using UnityEngine;

namespace AbilitySystem.Authoring
{
	/// <summary>
	/// Simple Ability that applies a Gameplay Effect to the activating character
	/// </summary>
	[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Single Target Ability")]
	public class SingleTargetAbilityScriptableObject : AbstractAbilityScriptableObject
	{
		/// <summary>
		/// Gameplay Effect to apply
		/// </summary>
		public GameplayEffectScriptableObject GameplayEffect;

		/// <summary>
		/// Creates the Ability Spec, which is instantiated for each character.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
		{
			var spec = new SingleTargetAbilitySpec(this, owner);
			spec.Level = owner.Level;
			return spec;
		}

		/// <summary>
		/// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
		/// is instantiated for each character, we can store stateful data here.
		/// </summary>
		public class SingleTargetAbilitySpec : AbstractAbilitySpec
		{
			public SingleTargetAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) :
				base(abilitySO, owner)
			{
			}

			/// <summary>
			/// What to do when the ability is cancelled.  We don't care about there for this example.
			/// </summary>
			public override void CancelAbility()
			{
			}

			/// <summary>
			/// What happens when we activate the ability.
			/// 
			/// In this example, we apply the cost and cooldown, and then we apply the main
			/// gameplay effect
			/// </summary>
			/// <returns></returns>
			public override IEnumerator ActivateAbility()
			{
				// Apply cost and cooldown
				var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
				var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
				this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
				this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

				// Apply primary effect
				var target = this.Owner.GetComponent<PlayerAI>().SelectedTarget;
				var effectSpec =
					this.Owner.MakeOutgoingSpec((this.Ability as SingleTargetAbilityScriptableObject).GameplayEffect);
				target.GetComponent<AbilitySystemCharacter>().ApplyGameplayEffectSpecToSelf(effectSpec);

				yield return null;
			}

			/// <summary>
			/// Checks to make sure Gameplay Tags checks are met. 
			/// 
			/// Since the target is also the character activating the ability,
			/// we can just use Owner for all of them.
			/// </summary>
			/// <returns></returns>
			public override bool CheckGameplayTags()
			{
				return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
				       && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags)
				       && AscHasAllTags(Owner, this.Ability.AbilityTags.SourceTags.RequireTags)
				       && AscHasNoneTags(Owner, this.Ability.AbilityTags.SourceTags.IgnoreTags)
				       && AscHasAllTags(Owner, this.Ability.AbilityTags.TargetTags.RequireTags)
				       && AscHasNoneTags(Owner, this.Ability.AbilityTags.TargetTags.IgnoreTags);
			}

			/// <summary>
			/// Logic to execute before activating the ability.  We don't need to do anything here
			/// for this example.
			/// </summary>
			/// <returns></returns>
			protected override IEnumerator PreActivate()
			{
				yield return null;
			}
		}
	}
}