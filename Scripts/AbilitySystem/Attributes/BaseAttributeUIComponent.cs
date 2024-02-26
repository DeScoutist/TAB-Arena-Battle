using UnityEngine;

namespace AbilitySystem.Attributes
{
    public abstract class BaseAttributeUIComponent : MonoBehaviour
    {
        public abstract void SetAttributeValue(float currentValue, float maxValue);
    }
}