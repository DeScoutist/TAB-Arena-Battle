using System;
using AbilitySystem.Authoring;
using UnityEngine;

namespace AbilitySystem.Abilities.Initial_Stats
{
	public class InitialStatsBehaviour : MonoBehaviour
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