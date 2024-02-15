// IAura.cs

using AbilitySystem.Authoring;
using UnityEngine;

namespace AbilitySystem.Abilities.Auras
{
	public enum AuraTargets
	{
		Enemy,
		Own,
		All,
		Specific
	}

	public interface IAura 
	{
		[SerializeField] AuraTargets Targets { get; set; }
		[SerializeField] GameplayEffectScriptableObject Effect { get; set; }
		[SerializeField] float Radius { get; set; }

		void Update();
	}
}