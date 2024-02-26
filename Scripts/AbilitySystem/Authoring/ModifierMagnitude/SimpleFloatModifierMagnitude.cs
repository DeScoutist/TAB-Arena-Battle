using UnityEngine;

namespace AbilitySystem.Authoring.ModifierMagnitude
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect/Modifier Magnitude/Simple Float")]
    public class SimpleFloatModifierMagnitude : ModifierMagnitudeScriptableObject
    {
        [SerializeField]
        private AnimationCurve ScalingFunction;

        public override void Initialise(GameplayEffectSpec spec)
        {
        }
        public override float? CalculateMagnitude(GameplayEffectSpec spec)
        {
            return ScalingFunction.Evaluate(spec.Level);
        }
    }
}
