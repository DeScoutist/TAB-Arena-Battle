using System;
using AbilitySystem.Authoring;
using GameplayTags.Authoring;

namespace AbilitySystem
{
    [Serializable]
    public struct ConditionalGameplayEffectContainer
    {
        public GameplayEffectScriptableObject GameplayEffect;
        public GameplayTagScriptableObject[] RequiredSourceTags;
    }

}
