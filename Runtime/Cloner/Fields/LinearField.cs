using Unity.Mathematics;
using UnityEngine;

namespace Misaki.ArtTool
{
    [ExecuteInEditMode]
    public class LinearField : FieldBase
    {
        public RemappingSetting remappingSetting = new();

        public float length = 1.0f;

        private float3 fieldForward;
        private float3 fieldPosition;

        public override void Initialize()
        {
            fieldForward = transform.forward;
            fieldPosition = transform.position;
        }

        public override float Operate(float3 position, float weight)
        {
            var plane = new Unity.Mathematics.Geometry.Plane(fieldForward, fieldPosition);
            var distance = plane.SignedDistanceToPoint(position) / length;

            weight = math.saturate(distance / 2.0f + 0.5f);
            weight = FieldHelper.ApplyRemapping(weight, ref remappingSetting);

            return weight;
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            var end = Vector3.forward * length;
            var start = Vector3.forward * -length;

            Gizmos.DrawLine(start, end);
            var right = Quaternion.LookRotation(Vector3.forward) * Quaternion.Euler(0.0f, 180.0f + 30.0f, 0.0f) * Vector3.forward;
            var left = Quaternion.LookRotation(Vector3.forward) * Quaternion.Euler(0.0f, 180.0f - 30.0f, 0.0f) * Vector3.forward;

            Gizmos.DrawLine(end, end + right * 0.5f);
            Gizmos.DrawLine(end, end + left * 0.5f);

            Gizmos.DrawWireCube(end, Vector3.one);
            Gizmos.DrawWireCube(start, Vector3.one);
        }
    }
}