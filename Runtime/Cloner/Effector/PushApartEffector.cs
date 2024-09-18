using System;
using Unity.Mathematics;

namespace Misaki.ArtTool
{
    public class PushApartEffector : EffectorBase
    {
        public float radius = 1.0f;
        public uint iteration = 10;
        public bool isHideMode = false;

        // TODO: Average the push direction and distance of each point for more consistence result
        public override PointData Operate(int index, float4x4 nodeWorldMatrix, ReadOnlySpan<PointData> points)
        {
            var currentPoint = points[index];

            var weight = CalculateFieldsWeight(currentPoint.matrix.c3.xyz);
            if (weight > 0)
            {
                for (var i = 0; i < iteration; i++)
                {
                    for (var p = 0; p < points.Length; p++)
                    {
                        if (index == p)
                        {
                            continue;
                        }

                        var targetPoint = points[p];

                        var distance = math.distance(currentPoint.matrix.c3.xyz, targetPoint.matrix.c3.xyz);
                        if (distance < radius)
                        {
                            var direction = math.normalizesafe(currentPoint.matrix.c3.xyz - targetPoint.matrix.c3.xyz);
                            currentPoint.matrix.c3.xyz += (radius - distance) * weight * direction;

                            //Debug.Log($"Push at index {index} with distance {radius - distance} and direction {direction}");
                        }
                    }
                }
            }

            return currentPoint;
        }

    }
}