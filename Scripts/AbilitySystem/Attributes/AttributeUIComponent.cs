using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace AbilitySystem.Attributes
{
    public class AttributeUIComponent : BaseAttributeUIComponent
    {
        [SerializeField]
        private Slider slider;

        [SerializeField]
        private float lerpSpeed;

        public override void SetAttributeValue(float currentValue, float maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = math.lerp(slider.value, currentValue, Time.deltaTime * lerpSpeed);
        }
    }
}
