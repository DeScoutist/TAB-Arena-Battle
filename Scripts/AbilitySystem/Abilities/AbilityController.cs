using AbilitySystem.Authoring;
using AbilitySystem.Components;
using UnityEngine;
using UnityEngine.UI;

namespace AbilitySystem.Abilities
{
    public class AbilityController : MonoBehaviour
    {
        public AbstractAbilityScriptableObject[] Abilities;

        public AbstractAbilityScriptableObject[] InitialisationAbilities;
        private AbilitySystemCharacter abilitySystemCharacter;

        public AbstractAbilitySpec[] abilitySpecs;

        public Image[] Cooldowns;

        // Start is called before the first frame update
        void Start()
        {
            this.abilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
            ActivateInitialisationAbilities();
        
            if (Abilities.Length == 0)
            {
                return;
            }
        
            var spec = Abilities[0].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            GrantCastableAbilities();
        }

        // Update is called once per frame
        void Update()
        {
            for (var i = 0; i < Cooldowns.Length; i++)
            {
                var durationRemaining = this.abilitySpecs[i].CheckCooldown();
                if (durationRemaining.TotalDuration > 0)
                {
                    var percentRemaining = durationRemaining.TimeRemaining / durationRemaining.TotalDuration;
                    Cooldowns[i].fillAmount = 1 - percentRemaining;
                }
                else
                {
                    Cooldowns[i].fillAmount = 1;
                }
            }
        }

        void ActivateInitialisationAbilities()
        {
            for (var i = 0; i < InitialisationAbilities.Length; i++)
            {
                var spec = InitialisationAbilities[i].CreateSpec(this.abilitySystemCharacter);
                Debug.Log($"{this.gameObject.name} tries to activate Initialisation ability {spec.Ability.name}");
                this.abilitySystemCharacter.GrantAbility(spec);
                StartCoroutine(spec.TryActivateAbility());
            }
        }

        void GrantCastableAbilities()
        {
            this.abilitySpecs = new AbstractAbilitySpec[Abilities.Length];
            for (var i = 0; i < Abilities.Length; i++)
            {
                var spec = Abilities[i].CreateSpec(this.abilitySystemCharacter);
                this.abilitySystemCharacter.GrantAbility(spec);
                this.abilitySpecs[i] = spec;
            }
        }

        public void UseAbility(int i)
        {
            var spec = abilitySpecs[i];
            StartCoroutine(spec.TryActivateAbility());
        }
    }
}
