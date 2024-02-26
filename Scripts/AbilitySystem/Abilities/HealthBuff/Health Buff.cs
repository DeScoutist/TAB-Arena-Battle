using AbilitySystem.Authoring;
using AbilitySystem.Components;
using UnityEngine;

namespace AbilitySystem.Abilities.HealthBuff
{
	public class HealthBuff : MonoBehaviour
	{
		[SerializeField] private AbstractAbilityScriptableObject ability;
		[SerializeField] private AbilitySystemCharacter abilitySystemCharacter;

		private void Awake()
		{
			var abilitySpec = ability.CreateSpec(abilitySystemCharacter);
			StartCoroutine(abilitySpec.TryActivateAbility());
		}
	}
}
