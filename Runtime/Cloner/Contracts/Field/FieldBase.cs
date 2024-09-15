using System;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public abstract class FieldBase : MonoBehaviour
    {
        public RemappingSetting remappingSetting = new();

        public EventHandler propertyChanged;

        public virtual void Initialize()
        {
        }

        public abstract float Operate(float3 position);

        protected float Remapping(float weight)
        {
            if (!remappingSetting.enable)
            {
                return weight;
            }

            weight = math.saturate(weight / (1.0f - remappingSetting.innerOffset));
            weight = math.lerp(remappingSetting.min, remappingSetting.max, weight);
            weight = remappingSetting.invert ? 1.0f - weight : weight;
            weight *= remappingSetting.strength;
            return weight;
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                propertyChanged.Invoke(this, null);
                transform.hasChanged = false;
            }
        }
    }
}