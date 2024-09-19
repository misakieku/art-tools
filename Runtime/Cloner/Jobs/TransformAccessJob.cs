using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Misaki.ArtTool
{
    public struct TransformAccessJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeList<float4x4> points;

        public void Execute(int index, TransformAccess transform)
        {
            var currentPoint = points[index];

            if (index > points.Length || !transform.isValid)
            {
                return;
            }

            MatrixHelper.DecomposeMatrix(currentPoint, out var position, out var rotation, out var scale);

            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = scale;
        }
    }
}