using System;
using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public abstract class FieldBase : MonoBehaviour
    {
        public EventHandler propertyChanged;

        public virtual void Initialize()
        {
        }

        public abstract float Operate(float3 position, float weight);

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