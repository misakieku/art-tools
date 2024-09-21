using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    public class SphereField : FieldBase
    {
        public RemappingSetting remappingSetting = new();

        public float radius = 1.0f;

        private float3 fieldPosition;

        public override void Initialize()
        {
            fieldPosition = transform.position;
        }

        public override float Operate(float3 position, float weight)
        {
            weight = ShapeHelper.Linear01DistanceToSphereCenter(position, fieldPosition, radius);
            weight = FieldHelper.ApplyRemapping(weight, ref remappingSetting);

            return weight;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(fieldPosition, radius);
        }
    }
}