using System.Collections;
using AbilitySystem.Abilities;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    /// <summary>
    /// Simple Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/AOEAbility")]
    public class AOEAbilityScriptableObject : AbstractAbilityScriptableObject
    {
        /// <summary>
        /// Gameplay Effect to apply
        /// </summary>
        public GameplayEffectScriptableObject GameplayEffect;
        
        public GameObject AoeMarker;

        public bool hasMarker = false;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new AOEAbilitySpec(this, owner, AoeMarker);
            spec.Level = owner.Level;
            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// </summary>
        public class AOEAbilitySpec : AbstractAbilitySpec
        {
            private GameObject AoeMarkerPrefab;
            private GameObject AoeMarker;
            
            public AOEAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner, GameObject aoeMarker) : base(abilitySO, owner)
            {
                AoeMarkerPrefab = aoeMarker;
            }

            /// <summary>
            /// What to do when the ability is cancelled.  We don't care about there for this example.
            /// </summary>
            public override void CancelAbility()
            {
                Destroy(AoeMarker);
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
                var targetedPlayers = AoeMarker.GetComponent<AbilityArea>().GetPlayersInArea();
                // Apply cost and cooldown
                var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);
                
                // Apply primary effect
                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as AOEAbilityScriptableObject).GameplayEffect);
                this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);
                
                foreach (var player in targetedPlayers)
                {
                    player.ApplyGameplayEffectSpecToSelf(effectSpec);
                }
                
                Destroy(AoeMarker);
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
                var owner = Owner.GetComponent<BossAI>();
                var position = owner.Target.transform.position;
                
                AoeMarker = Instantiate(AoeMarkerPrefab, position, Quaternion.identity);
                // AoeMarker.GetComponent<Rigidbody>().isKinematic = true;
                // AoeMarker.GetComponent<Rigidbody>().useGravity = false;
                Owner.GetComponent<UnitUI>().StartCoroutine(AoeMarkerAnimationShow(Ability.CastTime));
                yield return null;
            }
            
            public IEnumerator AoeMarkerAnimationShow(float currentCastTime)
            {
                var fill = AoeMarker.transform.Find("Fill");

                for (float t = 0.0f; t < currentCastTime; t += Time.deltaTime)
                {
                    var normalizedTime = t / currentCastTime;
                    var scaleX = Mathf.Lerp(1, 9.8f, normalizedTime);
                    var scaleZ = Mathf.Lerp(1, 9.8f, normalizedTime);
                    fill.transform.localScale = new Vector3(scaleX, fill.transform.localScale.y, scaleZ);

                    yield return null;
                }
            }
        }
    }

}