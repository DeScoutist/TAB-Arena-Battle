using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect Definition")]
    public class GameplayEffectScriptableObject : ScriptableObject
    {
        [SerializeField]
        public GameplayEffectDefinitionContainer gameplayEffect;

        [SerializeField]
        public GameplayEffectTags gameplayEffectTags;

        [SerializeField]
        public GameplayEffectPeriod Period;
        
        [SerializeField]
        public Sprite Icon; // Иконка для GameplayEffect

        [SerializeField]
        public float MaxStacks = 1;
        
        [SerializeField]
        public bool StacksRenewing = true;
        
        [SerializeField]
        public bool RefreshingTickPeriod = true;
    }
}