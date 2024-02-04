using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

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
