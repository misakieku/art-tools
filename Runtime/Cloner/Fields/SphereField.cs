using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public class SphereField : FieldBase
    {
        public float radius = 1.0f;

        private float3 fieldPosition;

        public override void Initialize()
        {
            fieldPosition = transform.position;
        }

        public override float Operate(float3 position)
        {
            var weight = ShapeHelper.Linear01DistanceToSphereCenter(position, fieldPosition, radius);
            weight = Remapping(weight);

            return weight;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(fieldPosition, radius);
        }
    }
}