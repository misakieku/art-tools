using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Misaki.ArtTool
{
    public struct TransformAccessJob : IJobParallelForTransform
    {
        public NativeArray<float4x4> points;

        public void Execute(int index, TransformAccess transform)
        {
            if (index > points.Length || !transform.isValid)
            {
                return;
            }

            MatrixHelper.DecomposeMatrix(points[index], out var position, out var rotation, out var scale);

            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = scale;
        }
    }
}